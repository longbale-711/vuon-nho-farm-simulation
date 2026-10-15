using System.Numerics;
using NUnit.Framework;
using Orchard.Currency;

namespace Orchard.Tests
{
    public class CurrencyTests
    {
        [Test]
        public void CurrencyExceedsDoubleRangeAndKeepsSingleCent()
        {
            var huge = new Money(BigInteger.Pow(10, 500));
            var changed = huge + new Money(1);
            Assert.AreEqual(BigInteger.One, (changed - huge).Cents);
            Assert.AreEqual(changed, Money.Parse(changed.Serialize()));
            StringAssert.Contains("e", changed.Display());
        }

        [TestCase(150, "150")]
        [TestCase(1250, "1.25K")]
        [TestCase(1000000, "1M")]
        public void CurrencyFormatting(long coins, string expected) => Assert.AreEqual(expected, Money.Coins(coins).Display());
    }
}
