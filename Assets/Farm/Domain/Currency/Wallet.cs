using System;

namespace Orchard.Currency
{
    public sealed class Wallet
    {
        public Money Balance { get; private set; } = Money.Coins(150);
        public Money TotalEarned
        {
            get; private set;
        }
        public int Sales
        {
            get; private set;
        }

        public bool TrySpend(Money cost)
        {
            if (cost.Cents < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cost));
            }

            if (Balance < cost)
            {
                return false;
            }

            Balance -= cost;
            return true;
        }

        public void ReceivePayment(Money payment)
        {
            if (payment.Cents < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(payment));
            }

            Balance += payment;
            TotalEarned += payment;
            Sales++;
        }

        internal void Restore(Money balance, Money totalEarned, int sales)
        {
            Balance = balance;
            TotalEarned = totalEarned;
            Sales = sales;
        }
    }
}
