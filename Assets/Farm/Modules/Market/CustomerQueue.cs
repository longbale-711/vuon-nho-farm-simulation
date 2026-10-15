using System.Collections.Generic;

namespace Orchard.Market
{
    public sealed class CustomerQueue
    {
        private readonly IReadOnlyList<CustomerAgent> _customers;
        private long _arrivalOrder;
        public CustomerQueue(IReadOnlyList<CustomerAgent> customers)
        {
            _customers = customers;
        }

        public long NextArrivalOrder() => ++_arrivalOrder;
        public CustomerAgent ReserveCustomer()
        {
            CustomerAgent oldest = null;
            foreach (CustomerAgent customer in _customers)
            {
                if (customer.CanServe && (oldest == null || customer.ArrivalOrder < oldest.ArrivalOrder))
                {
                    oldest = customer;
                }
            }

            return oldest != null && oldest.Reserve() ? oldest : null;
        }

        public void Reorder(int lane, CustomerAgent returning)
        {
            var queue = new List<CustomerAgent>();
            foreach (CustomerAgent customer in _customers)
            {
                if (customer.Lane == lane && customer != returning && IsQueued(customer.State))
                {
                    queue.Add(customer);
                }
            }

            queue.Sort((left, right) => left.ArrivalOrder.CompareTo(right.ArrivalOrder));
            if (returning != null)
            {
                queue.Add(returning);
            }

            for (int rank = 0; rank < queue.Count; rank++)
            {
                queue[rank].SetQueueRank(rank);
            }
        }

        private static bool IsQueued(CustomerState state)
        {
            return state == CustomerState.Arriving || state == CustomerState.Waiting || state == CustomerState.Reserved;
        }
    }
}
