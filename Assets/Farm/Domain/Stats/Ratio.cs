using System;
using System.Numerics;
using Orchard.Currency;

namespace Orchard.Stats
{
    public readonly struct Ratio
    {
        public BigInteger Numerator
        {
            get;
        }
        public BigInteger Denominator
        {
            get;
        }

        public Ratio(BigInteger numerator, BigInteger denominator)
        {
            if (numerator < 0 || denominator <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            var gcd = BigInteger.GreatestCommonDivisor(numerator, denominator);
            Numerator = numerator / gcd;
            Denominator = denominator / gcd;
        }

        public static Ratio One => new Ratio(1, 1);

        public static Ratio operator *(Ratio left, Ratio right) => new Ratio(left.Numerator * right.Numerator, left.Denominator * right.Denominator);
        public Money Apply(Money value) => value.Scale(Numerator, Denominator);
        public float AsFloat() => (float)((double)Numerator / (double)Denominator);
    }
}
