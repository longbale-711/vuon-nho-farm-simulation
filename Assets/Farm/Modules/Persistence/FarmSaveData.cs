using System;

namespace Orchard.Persistence
{
    [Serializable]
    internal sealed class FarmSaveData
    {
        public int Version = 1;
        public string Balance;
        public string Earned;
        public int Sales;
        public int GlobalProfit;
        public int Customers;
        public int Speed;
        public int Workers;
        public int[] Levels;
        public int[] ProfitLevels;
        public FarmSave ToSnapshot()
        {
            return new FarmSave
            {
                Version = Version,
                Balance = Balance,
                Earned = Earned,
                Sales = Sales,
                GlobalProfit = GlobalProfit,
                Customers = Customers,
                Speed = Speed,
                Workers = Workers,
                Levels = Levels,
                ProfitLevels = ProfitLevels
            };
        }

        public static FarmSaveData FromSnapshot(FarmSave save)
        {
            return new FarmSaveData
            {
                Version = save.Version,
                Balance = save.Balance,
                Earned = save.Earned,
                Sales = save.Sales,
                GlobalProfit = save.GlobalProfit,
                Customers = save.Customers,
                Speed = save.Speed,
                Workers = save.Workers,
                Levels = save.Levels,
                ProfitLevels = save.ProfitLevels
            };
        }
    }
}
