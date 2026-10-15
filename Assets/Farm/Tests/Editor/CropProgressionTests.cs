using NUnit.Framework;
using Orchard.Currency;
using Orchard.Economy;
using Orchard.Management;

namespace Orchard.Tests
{
    public class CropProgressionTests
    {
        [Test]
        public void FirstBuildCannotSpendTwiceOrMakeBalanceNegative()
        {
            var economy = new FarmEconomy();
            Assert.IsTrue(economy.Build(0));
            Assert.AreEqual(Money.Coins(50), economy.Balance);
            Assert.IsFalse(economy.Build(0));
            Assert.IsFalse(economy.Build(1));
            Assert.AreEqual(Money.Coins(50), economy.Balance);
        }

        [Test]
        public void PlantLevelsFollowTenPercentOfBasePrice()
        {
            var economy = TestEconomyFactory.CreateFunded();
            economy.Build(0);
            Assert.AreEqual(Money.Coins(20), economy.HarvestPrice(0));
            economy.Upgrade(0);
            Assert.AreEqual(Money.Coins(22), economy.HarvestPrice(0));
            economy.Upgrade(0);
            Assert.AreEqual(Money.Coins(24), economy.HarvestPrice(0));
        }

        [Test]
        public void LocalAndGlobalBuffsStackWithoutAffectingOtherLocalPlants()
        {
            var economy = TestEconomyFactory.CreateFunded();
            economy.Build(0);
            economy.Build(1);
            economy.Purchase(ManagementUpgrade.FirstCropProfit);
            economy.Purchase(ManagementUpgrade.GlobalProfit);
            Assert.AreEqual(Money.Coins(80), economy.HarvestPrice(0));
            Assert.AreEqual(Money.Coins(80), economy.HarvestPrice(1));
        }

        [Test]
        public void SaleUsesPriceAtHarvestEvenAfterUpgrades()
        {
            var economy = TestEconomyFactory.CreateFunded();
            economy.Build(0);
            var cargo = economy.HarvestPrice(0);
            economy.Upgrade(0);
            economy.Purchase(ManagementUpgrade.GlobalProfit);
            var before = economy.Balance;
            economy.Sell(cargo);
            Assert.AreEqual(Money.Coins(20), economy.Balance - before);
            Assert.AreEqual(Money.Coins(44), economy.HarvestPrice(0));
        }
    }
}
