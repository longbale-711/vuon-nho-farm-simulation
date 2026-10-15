using System.Collections.Generic;

namespace Orchard.Crops
{
    public static class CropCatalog
    {
        private static readonly IReadOnlyList<CropDefinition> s_definitions = System.Array.AsReadOnly(new[] { new CropDefinition(100, 20), new CropDefinition(300, 40), new CropDefinition(800, 80), new CropDefinition(2000, 160), new CropDefinition(5000, 320), new CropDefinition(12000, 640) });
        public const int PlotCount = 6;
        public const int MaximumLevel = 10000;
        public const int MaximumProfitUpgrade = 1000;
        public const float GrowthSeconds = 3.5f;
        public static CropDefinition GetDefinition(int index) => s_definitions[index];
    }
}
