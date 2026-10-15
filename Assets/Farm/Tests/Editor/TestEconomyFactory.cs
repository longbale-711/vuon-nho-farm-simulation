using Orchard.Currency;
using Orchard.Economy;

namespace Orchard.Tests
{
    internal static class TestEconomyFactory
    {
        public static FarmEconomy CreateFunded()
        {
            var economy = new FarmEconomy();
            economy.Sell(Money.Coins(1000000000));
            return economy;
        }
    }
}
