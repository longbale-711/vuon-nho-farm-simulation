using Orchard.Currency;

namespace Orchard.Crops
{
    public sealed class CropDefinition
    {
        public Money BuildCost
        {
            get;
        }
        public Money BaseHarvestPrice
        {
            get;
        }

        public CropDefinition(long buildCost, long baseHarvestPrice)
        {
            BuildCost = Money.Coins(buildCost);
            BaseHarvestPrice = Money.Coins(baseHarvestPrice);
        }
    }
}
