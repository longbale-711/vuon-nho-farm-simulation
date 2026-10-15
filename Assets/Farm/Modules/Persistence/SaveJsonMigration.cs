using System.Text.RegularExpressions;

namespace Orchard.Persistence
{
    internal static class SaveJsonMigration
    {
        private static readonly Regex s_legacyProperty = new Regex("(?<prefix>[{,]\\s*)\"(?<name>version|balance|earned|sales|globalProfit|customers|speed|workers|levels|profitLevels)\"(?=\\s*:)", RegexOptions.CultureInvariant);
        public static string Normalize(string json)
        {
            // Match schema keys after object separators, not currency values or array entries.
            return s_legacyProperty.Replace(json, match =>
            {
                string name = match.Groups["name"].Value;
                string propertyName = char.ToUpperInvariant(name[0]) + name.Substring(1);
                return match.Groups["prefix"].Value + "\"" + propertyName + "\"";
            });
        }
    }
}
