using System;
using System.Collections.Generic;
using System.Numerics;
using Orchard.Currency;
using Orchard.Stats;

namespace Orchard.Crops
{
    public sealed class CropProgression
    {
        private readonly CropAccount[] _accounts = new CropAccount[CropCatalog.PlotCount];
        public IReadOnlyList<CropAccount> Accounts
        {
            get;
        }

        public CropProgression()
        {
            for (int index = 0; index < _accounts.Length; index++)
            {
                _accounts[index] = new CropAccount();
            }

            Accounts = Array.AsReadOnly(_accounts);
        }

        public Money GetBuildCost(int index) => CropCatalog.GetDefinition(index).BuildCost;
        public Money GetUpgradeCost(int index)
        {
            int level = _accounts[index].Level;
            return GetBuildCost(index).Scale(BigInteger.Pow(12, level), BigInteger.Pow(10, level));
        }

        public Money GetHarvestPrice(int index, Ratio globalProfit, bool previewNextLevel = false)
        {
            CropAccount account = _accounts[index];
            if (!account.Built)
            {
                return Money.Coins(0);
            }

            int level = account.Level + (previewNextLevel ? 1 : 0);
            // Apply all rational modifiers before rounding once to the smallest currency unit.
            Ratio multiplier = new Ratio(10 + level - 1, 10) * account.Profit.Multiplier * globalProfit;
            return multiplier.Apply(CropCatalog.GetDefinition(index).BaseHarvestPrice);
        }

        public bool TryBuild(int index, Wallet wallet)
        {
            CropAccount account = _accounts[index];
            if (account.Built || !wallet.TrySpend(GetBuildCost(index)))
            {
                return false;
            }

            account.Level = 1;
            return true;
        }

        public bool TryUpgrade(int index, Wallet wallet)
        {
            CropAccount account = _accounts[index];
            if (!account.Built || account.Level >= CropCatalog.MaximumLevel || !wallet.TrySpend(GetUpgradeCost(index)))
            {
                return false;
            }

            account.Level++;
            return true;
        }

        internal void IncreaseProfit(int index)
        {
            _accounts[index].ProfitUpgrade++;
            RefreshProfit(index);
        }

        internal void Restore(int[] levels, int[] profitLevels)
        {
            for (int index = 0; index < _accounts.Length; index++)
            {
                _accounts[index].Level = levels[index];
                _accounts[index].ProfitUpgrade = profitLevels[index];
                RefreshProfit(index);
            }
        }

        private void RefreshProfit(int index)
        {
            CropAccount account = _accounts[index];
            account.Profit.Set("management.local", new Ratio(BigInteger.Pow(2, account.ProfitUpgrade), 1));
        }
    }
}
