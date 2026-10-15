using System;
using Orchard.Crops;

namespace Orchard.Persistence
{
    [Serializable]
    public sealed class FarmSave
    {
        public int Version = 1;
        public string Balance = "15000";
        public string Earned = "0";
        public int Sales;
        public int GlobalProfit;
        public int Customers;
        public int Speed;
        public int Workers;
        public int[] Levels = new int[CropCatalog.PlotCount];
        public int[] ProfitLevels = new int[CropCatalog.PlotCount];
    }
}
