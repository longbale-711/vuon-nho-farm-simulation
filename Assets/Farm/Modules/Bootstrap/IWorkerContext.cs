using Orchard.Crops;
using Orchard.Currency;
using Orchard.Economy;
using Orchard.Market;
using Orchard.Navigation;
using UnityEngine;

namespace Orchard.Bootstrap
{
    public interface IWorkerContext
    {
        FarmEconomy Economy
        {
            get;
        }

        FarmNavigation Navigation
        {
            get;
        }

        FarmResources Resources
        {
            get;
        }

        CropPlot ReserveCrop(Vector3 position);
        CustomerAgent ReserveCustomer();
        void Effect(GameObject prefab, Vector3 position);
        void ShowPayment(Vector3 position, Money payment);
    }
}
