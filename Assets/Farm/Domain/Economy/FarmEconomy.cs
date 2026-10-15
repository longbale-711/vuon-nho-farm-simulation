using System;
using System.Collections.Generic;
using Orchard.Crops;
using Orchard.Currency;
using Orchard.Management;
using Orchard.Persistence;
using Orchard.Stats;

namespace Orchard.Economy
{
    public sealed class FarmEconomy
    {
        public const int PlotCount = CropCatalog.PlotCount;
        public const int MaxCropLevel = CropCatalog.MaximumLevel;
        public const int MaxManagementLevel = CropCatalog.MaximumProfitUpgrade;
        private readonly Wallet _wallet = new Wallet();
        private readonly CropProgression _cropProgression = new CropProgression();
        private readonly ManagementProgression _managementProgression;
        public Money Balance => _wallet.Balance;
        public Money TotalEarned => _wallet.TotalEarned;
        public int Sales => _wallet.Sales;
        public int CustomerCount => _managementProgression.CustomerCount;
        public int WorkerCount => _managementProgression.WorkerCount;
        public IReadOnlyList<CropAccount> Crops => _cropProgression.Accounts;
        public StatModifiers GlobalProfit => _managementProgression.GlobalProfit;
        public StatModifiers WorkerSpeed => _managementProgression.WorkerSpeed;

        public event Action Changed;
        public FarmEconomy()
        {
            _managementProgression = new ManagementProgression(_cropProgression);
        }

        public Money BuildCost(int plot) => _cropProgression.GetBuildCost(plot);
        public Money UpgradeCost(int plot) => _cropProgression.GetUpgradeCost(plot);
        public Money HarvestPrice(int plot) => _cropProgression.GetHarvestPrice(plot, GlobalProfit.Multiplier);
        public Money NextHarvestPrice(int plot) => _cropProgression.GetHarvestPrice(plot, GlobalProfit.Multiplier, true);
        public int ManagementLevel(ManagementUpgrade upgrade) => _managementProgression.GetLevel(upgrade);
        public bool ManagementAvailable(ManagementUpgrade upgrade) => _managementProgression.CanPurchase(upgrade);
        public Money ManagementCost(ManagementUpgrade upgrade) => _managementProgression.GetCost(upgrade);
        public bool Build(int plot)
        {
            return PublishChange(_cropProgression.TryBuild(plot, _wallet));
        }

        public bool Upgrade(int plot)
        {
            return PublishChange(_cropProgression.TryUpgrade(plot, _wallet));
        }

        public bool Purchase(ManagementUpgrade upgrade)
        {
            return PublishChange(_managementProgression.TryPurchase(upgrade, _wallet));
        }

        public void Sell(Money harvestedValue)
        {
            _wallet.ReceivePayment(harvestedValue);
            Changed?.Invoke();
        }

        public FarmSave Snapshot()
        {
            var save = new FarmSave
            {
                Balance = Balance.Serialize(),
                Earned = TotalEarned.Serialize(),
                Sales = Sales,
                GlobalProfit = ManagementLevel(ManagementUpgrade.GlobalProfit),
                Customers = ManagementLevel(ManagementUpgrade.Customers),
                Speed = ManagementLevel(ManagementUpgrade.WorkerSpeed),
                Workers = ManagementLevel(ManagementUpgrade.ExtraWorker)
            };
            for (int index = 0; index < PlotCount; index++)
            {
                save.Levels[index] = Crops[index].Level;
                save.ProfitLevels[index] = Crops[index].ProfitUpgrade;
            }

            return save;
        }

        public bool Restore(FarmSave save)
        {
            if (!FarmSaveValidator.IsValid(save))
            {
                return false;
            }

            try
            {
                Money balance = Money.Parse(save.Balance);
                Money earned = Money.Parse(save.Earned);
                if (balance.Cents < 0 || earned.Cents < 0)
                {
                    return false;
                }

                _wallet.Restore(balance, earned, save.Sales);
                _cropProgression.Restore(save.Levels, save.ProfitLevels);
                _managementProgression.Restore(save.GlobalProfit, save.Customers, save.Speed, save.Workers);
                Changed?.Invoke();
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        private bool PublishChange(bool succeeded)
        {
            if (succeeded)
            {
                Changed?.Invoke();
            }

            return succeeded;
        }
    }
}
