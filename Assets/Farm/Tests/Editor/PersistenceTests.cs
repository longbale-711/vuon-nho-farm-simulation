using System;
using NUnit.Framework;
using Orchard.Currency;
using Orchard.Economy;
using Orchard.Management;

namespace Orchard.Tests
{
    public class PersistenceTests
    {
        [Test]
        public void SnapshotRestoresAllPurchasesExactly()
        {
            var economy = TestEconomyFactory.CreateFunded();
            economy.Build(0);
            economy.Upgrade(0);
            foreach (ManagementUpgrade upgrade in Enum.GetValues(typeof(ManagementUpgrade)))
            {
                economy.Purchase(upgrade);
            }

            var restored = new FarmEconomy();
            Assert.IsTrue(restored.Restore(economy.Snapshot()));
            Assert.AreEqual(economy.Balance, restored.Balance);
            Assert.AreEqual(economy.CustomerCount, restored.CustomerCount);
            Assert.AreEqual(economy.HarvestPrice(0), restored.HarvestPrice(0));
            Assert.AreEqual(economy.WorkerCount, restored.WorkerCount);
        }

        [Test]
        public void InvalidSaveDoesNotPartiallyMutateEconomy()
        {
            var economy = new FarmEconomy();
            var data = TestEconomyFactory.CreateFunded().Snapshot();
            data.Levels[5] = -1;
            Assert.IsFalse(economy.Restore(data));
            Assert.AreEqual(Money.Coins(150), economy.Balance);
            data.Levels[5] = 0;
            data.Balance = "broken";
            Assert.IsFalse(economy.Restore(data));
        }
    }
}
