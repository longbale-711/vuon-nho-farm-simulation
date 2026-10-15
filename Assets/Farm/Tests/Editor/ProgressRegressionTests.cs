using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Orchard.Crops;
using Orchard.Currency;
using Orchard.Economy;
using Orchard.Management;
using Orchard.Persistence;

namespace Orchard.Tests
{
    public sealed class ProgressRegressionTests
    {
        private string _saveFolder;
        [SetUp]
        public void SetUp()
        {
            _saveFolder = Path.Combine(Path.GetTempPath(), "orchard-regression-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_saveFolder);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_saveFolder, true);
        }

        [Test]
        public void LegacySavePreservesCurrencyLevelsAndManagement()
        {
            string legacyJson = "{\"version\":1,\"balance\":\"123456789012345678901234\",\"earned\":\"9900\",\"sales\":7," + "\"globalProfit\":2,\"customers\":3,\"speed\":1,\"workers\":2," + "\"levels\":[3,1,0,0,0,0],\"profitLevels\":[2,0,0,0,0,0]}";
            File.WriteAllText(Path.Combine(_saveFolder, "orchard-save.json"), legacyJson);
            var economy = new FarmEconomy();
            Assert.IsTrue(new SaveStore(_saveFolder).Load(economy));
            Assert.AreEqual(Money.Parse("123456789012345678901234"), economy.Balance);
            Assert.AreEqual(3, economy.Crops[0].Level);
            Assert.AreEqual(7, economy.CustomerCount);
            Assert.AreEqual(4, economy.WorkerCount);
            Assert.AreEqual(Money.Coins(384), economy.HarvestPrice(0));
        }

        [Test]
        public void CorruptedPrimarySaveFallsBackToLastValidBackup()
        {
            var economy = new FarmEconomy();
            var store = new SaveStore(_saveFolder);
            Assert.IsTrue(store.Save(economy));
            economy.Build(0);
            Assert.IsTrue(store.Save(economy));
            File.WriteAllText(Path.Combine(_saveFolder, "orchard-save.json"), "{}");
            var restored = new FarmEconomy();
            Assert.IsTrue(store.Load(restored));
            Assert.AreEqual(Money.Coins(150), restored.Balance);
            Assert.IsFalse(restored.Crops[0].Built);
        }

        [Test]
        public void UpgradePreviewMatchesNextHarvestWithoutMutatingProgress()
        {
            var economy = new FarmEconomy();
            economy.Sell(Money.Coins(10000));
            economy.Build(0);
            economy.Purchase(ManagementUpgrade.FirstCropProfit);
            economy.Purchase(ManagementUpgrade.GlobalProfit);
            Money preview = economy.NextHarvestPrice(0);
            Assert.AreEqual(1, economy.Crops[0].Level);
            economy.Upgrade(0);
            Assert.AreEqual(preview, economy.HarvestPrice(0));
        }

        [Test]
        public void FailedTransactionDoesNotPublishProgressChange()
        {
            var economy = new FarmEconomy();
            int changes = 0;
            economy.Changed += () => changes++;
            Assert.IsFalse(economy.Build(1));
            Assert.AreEqual(0, changes);
            Assert.IsTrue(economy.Build(0));
            Assert.AreEqual(1, changes);
        }

        [Test]
        public void UnknownUpgradeCannotSpendCurrency()
        {
            var economy = new FarmEconomy();
            Assert.Throws<ArgumentOutOfRangeException>(() => economy.Purchase((ManagementUpgrade)99));
            Assert.AreEqual(Money.Coins(150), economy.Balance);
        }

        [Test]
        public void CropCollectionCannotBeReplacedByPresentation()
        {
            var economy = new FarmEconomy();
            var accounts = (IList<CropAccount>)economy.Crops;
            Assert.Throws<NotSupportedException>(() => accounts[0] = new CropAccount());
        }

        [Test]
        public void WalletRejectsNegativeCostsAndPayments()
        {
            var wallet = new Wallet();
            Assert.Throws<ArgumentOutOfRangeException>(() => wallet.TrySpend(Money.Coins(-1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => wallet.ReceivePayment(Money.Coins(-1)));
            Assert.AreEqual(Money.Coins(150), wallet.Balance);
        }
    }
}
