using System;
using System.Collections.Generic;

namespace Orchard.Stats
{
    public sealed class StatModifiers
    {
        private readonly SortedDictionary<string, Ratio> _sources = new SortedDictionary<string, Ratio>(StringComparer.Ordinal);
        public void Set(string source, Ratio multiplier) => _sources[source] = multiplier;
        public bool Remove(string source) => _sources.Remove(source);
        public Ratio Multiplier
        {
            get
            {
                var result = Ratio.One;
                foreach (var buff in _sources.Values)
                {
                    result *= buff;
                }

                return result;
            }
        }
    }
}
