using System;

namespace Orchard.Management
{
    public static class UpgradeCatalog
    {
        private static readonly UpgradeDefinition[] s_definitions =
        {
            new UpgradeDefinition(180, 1000),
            new UpgradeDefinition(500, 1000),
            new UpgradeDefinition(250, 5),
            new UpgradeDefinition(200, 10),
            new UpgradeDefinition(600, 4)
        };
        public static UpgradeDefinition GetDefinition(ManagementUpgrade upgrade)
        {
            int index = (int)upgrade;
            if (index < 0 || index >= s_definitions.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(upgrade));
            }

            return s_definitions[index];
        }
    }
}
