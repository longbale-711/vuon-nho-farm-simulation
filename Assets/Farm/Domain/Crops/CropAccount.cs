using Orchard.Stats;

namespace Orchard.Crops
{
    public sealed class CropAccount
    {
        public int Level
        {
            get; internal set;
        }
        public int ProfitUpgrade
        {
            get; internal set;
        }
        public bool Built => Level > 0;
        public StatModifiers Profit { get; } = new StatModifiers();
    }
}
