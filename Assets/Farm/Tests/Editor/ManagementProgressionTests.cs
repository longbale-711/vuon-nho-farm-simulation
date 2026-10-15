using NUnit.Framework;
using Orchard.Management;

namespace Orchard.Tests
{
    public class ManagementProgressionTests
    {
        [Test]
        public void StartsWithOneCustomerAndUpgradeAddsTwo()
        {
            var economy = TestEconomyFactory.CreateFunded();
            Assert.AreEqual(1, economy.CustomerCount);
            economy.Purchase(ManagementUpgrade.Customers);
            Assert.AreEqual(3, economy.CustomerCount);
        }

        [Test]
        public void ManagementCapsPreventUnlimitedActorGrowth()
        {
            var economy = TestEconomyFactory.CreateFunded();
            for (int i = 0; i < 5; i++)
            {
                Assert.IsTrue(economy.Purchase(ManagementUpgrade.Customers));
            }

            Assert.IsFalse(economy.Purchase(ManagementUpgrade.Customers));
            Assert.AreEqual(11, economy.CustomerCount);
        }
    }
}
