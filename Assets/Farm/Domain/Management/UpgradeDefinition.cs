namespace Orchard.Management
{
    public sealed class UpgradeDefinition
    {
        public long BaseCost
        {
            get;
        }
        public int MaximumLevel
        {
            get;
        }

        public UpgradeDefinition(long baseCost, int maximumLevel)
        {
            BaseCost = baseCost;
            MaximumLevel = maximumLevel;
        }
    }
}
