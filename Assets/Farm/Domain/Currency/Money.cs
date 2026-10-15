using System;
using System.Globalization;
using System.Numerics;

namespace Orchard.Currency
{
    public readonly struct Money : IComparable<Money>, IEquatable<Money>
    {
        public BigInteger Cents
        {
            get;
        }

        public Money(BigInteger cents)
        {
            Cents = cents;
        }

        public static Money Coins(long coins) => new Money(new BigInteger(coins) * 100);
        public static Money Parse(string value) => new Money(BigInteger.Parse(value, CultureInfo.InvariantCulture));
        public string Serialize() => Cents.ToString(CultureInfo.InvariantCulture);
        public Money Scale(BigInteger numerator, BigInteger denominator)
        {
            if (denominator <= 0 || numerator < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            return new Money(Cents * numerator / denominator);
        }

        public int CompareTo(Money other) => Cents.CompareTo(other.Cents);
        public bool Equals(Money other) => Cents.Equals(other.Cents);
        public override bool Equals(object otherObject) => otherObject is Money other && Equals(other);
        public override int GetHashCode() => Cents.GetHashCode();
        public static Money operator +(Money left, Money right) => new Money(left.Cents + right.Cents);
        public static Money operator -(Money left, Money right) => new Money(left.Cents - right.Cents);
        public static bool operator <(Money left, Money right) => left.Cents < right.Cents;
        public static bool operator >(Money left, Money right) => left.Cents > right.Cents;
        public static bool operator >=(Money left, Money right) => left.Cents >= right.Cents;
        public static bool operator <=(Money left, Money right) => left.Cents <= right.Cents;
        public static bool operator ==(Money left, Money right) => left.Equals(right);
        public static bool operator !=(Money left, Money right) => !left.Equals(right);
        public string Display()
        {
            var units = new[]
            {
                "",
                "K",
                "M",
                "B",
                "T",
                "Qa",
                "Qi",
                "Sx",
                "Sp",
                "Oc",
                "No",
                "Dc"
            };
            var value = BigInteger.Abs(Cents);
            var divisor = new BigInteger(100);
            int unit = 0;
            while (value >= divisor * 1000 && unit < units.Length - 1)
            {
                divisor *= 1000;
                unit++;
            }

            if (unit == units.Length - 1 && value >= divisor * 1000)
            {
                string digits = (value / 100).ToString(CultureInfo.InvariantCulture);
                return (Cents.Sign < 0 ? "-" : "") + digits[0] + "." + digits.Substring(1, 2) + "e" + (digits.Length - 1);
            }

            var whole = value / divisor;
            int hundredths = (int)(value % divisor * 100 / divisor);
            string fraction = hundredths == 0 ? "" : "." + hundredths.ToString("00").TrimEnd('0');
            return (Cents.Sign < 0 ? "-" : "") + whole + fraction + units[unit];
        }
    }
}
