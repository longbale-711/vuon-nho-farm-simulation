using Orchard.Market;
using Orchard.Navigation;

namespace Orchard.Bootstrap
{
    public interface ICustomerContext
    {
        FarmNavigation Navigation
        {
            get;
        }

        long NextArrivalOrder();
        void ReorderQueue(int lane, CustomerAgent returning = null);
    }
}
