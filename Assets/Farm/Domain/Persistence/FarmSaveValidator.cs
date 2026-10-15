using Orchard.Crops;
using Orchard.Management;

namespace Orchard.Persistence
{
    public static class FarmSaveValidator
    {
        public static bool IsValid(FarmSave save)
        {
            if (save == null || save.Version != 1 || save.Levels == null || save.Levels.Length != CropCatalog.PlotCount || save.ProfitLevels == null || save.ProfitLevels.Length != CropCatalog.PlotCount || save.Sales < 0)
            {
                return false;
            }

            if (!IsValidLevel(save.GlobalProfit, ManagementUpgrade.GlobalProfit) || !IsValidLevel(save.Customers, ManagementUpgrade.Customers) || !IsValidLevel(save.Speed, ManagementUpgrade.WorkerSpeed) || !IsValidLevel(save.Workers, ManagementUpgrade.ExtraWorker))
            {
                return false;
            }

            for (int index = 0; index < CropCatalog.PlotCount; index++)
            {
                if (save.Levels[index] < 0 || save.Levels[index] > CropCatalog.MaximumLevel || save.ProfitLevels[index] < 0 || save.ProfitLevels[index] > CropCatalog.MaximumProfitUpgrade)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsValidLevel(int level, ManagementUpgrade upgrade)
        {
            return level >= 0 && level <= UpgradeCatalog.GetDefinition(upgrade).MaximumLevel;
        }
    }
}
