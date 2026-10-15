using System.Numerics;
using Orchard.Crops;
using Orchard.Currency;
using Orchard.Stats;

namespace Orchard.Management
{
    public sealed class ManagementProgression
    {
        private readonly CropProgression _crops;
        private readonly int[] _levels = new int[5];
        public int CustomerCount => 1 + GetLevel(ManagementUpgrade.Customers) * 2;
        public int WorkerCount => 2 + GetLevel(ManagementUpgrade.ExtraWorker);
        public StatModifiers GlobalProfit { get; } = new StatModifiers();
        public StatModifiers WorkerSpeed { get; } = new StatModifiers();

        public ManagementProgression(CropProgression crops)
        {
            _crops = crops;
            RefreshModifiers();
        }

        public int GetLevel(ManagementUpgrade upgrade)
        {
            UpgradeCatalog.GetDefinition(upgrade);
            return upgrade == ManagementUpgrade.FirstCropProfit ? _crops.Accounts[0].ProfitUpgrade : _levels[(int)upgrade];
        }

        public bool CanPurchase(ManagementUpgrade upgrade)
        {
            UpgradeDefinition definition = UpgradeCatalog.GetDefinition(upgrade);
            return GetLevel(upgrade) < definition.MaximumLevel && (upgrade != ManagementUpgrade.FirstCropProfit || _crops.Accounts[0].Built);
        }

        public Money GetCost(ManagementUpgrade upgrade)
        {
            UpgradeDefinition definition = UpgradeCatalog.GetDefinition(upgrade);
            return Money.Coins(definition.BaseCost).Scale(BigInteger.Pow(3, GetLevel(upgrade)), 1);
        }

        public bool TryPurchase(ManagementUpgrade upgrade, Wallet wallet)
        {
            if (!CanPurchase(upgrade) || !wallet.TrySpend(GetCost(upgrade)))
            {
                return false;
            }

            if (upgrade == ManagementUpgrade.FirstCropProfit)
            {
                _crops.IncreaseProfit(0);
            }
            else
            {
                _levels[(int)upgrade]++;
            }

            RefreshModifiers();
            return true;
        }

        internal void Restore(int globalProfit, int customers, int speed, int workers)
        {
            _levels[(int)ManagementUpgrade.GlobalProfit] = globalProfit;
            _levels[(int)ManagementUpgrade.Customers] = customers;
            _levels[(int)ManagementUpgrade.WorkerSpeed] = speed;
            _levels[(int)ManagementUpgrade.ExtraWorker] = workers;
            RefreshModifiers();
        }

        private void RefreshModifiers()
        {
            GlobalProfit.Set("management.global", new Ratio(BigInteger.Pow(2, GetLevel(ManagementUpgrade.GlobalProfit)), 1));
            WorkerSpeed.Set("management.speed", new Ratio(10 + GetLevel(ManagementUpgrade.WorkerSpeed) * 2, 10));
        }
    }
}
