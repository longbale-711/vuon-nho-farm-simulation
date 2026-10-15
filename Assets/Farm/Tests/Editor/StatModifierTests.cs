using NUnit.Framework;
using Orchard.Currency;
using Orchard.Stats;

namespace Orchard.Tests
{
    public class StatModifierTests
    {
        [Test]
        public void BuffSourcesReplaceAndRemoveIndependently()
        {
            var buffs = new StatModifiers();
            buffs.Set("event", new Ratio(2, 1));
            buffs.Set("manager", new Ratio(3, 1));
            Assert.AreEqual(Money.Coins(60), buffs.Multiplier.Apply(Money.Coins(10)));
            buffs.Set("event", new Ratio(4, 1));
            buffs.Remove("manager");
            Assert.AreEqual(Money.Coins(40), buffs.Multiplier.Apply(Money.Coins(10)));
        }
    }
}
