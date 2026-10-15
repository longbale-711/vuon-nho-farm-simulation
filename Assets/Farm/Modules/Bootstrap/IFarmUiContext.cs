using System.Collections.Generic;
using Orchard.Crops;
using Orchard.Economy;
using UnityEngine;

namespace Orchard.Bootstrap
{
    public interface IFarmUiContext
    {
        FarmEconomy Economy
        {
            get;
        }

        FarmResources Resources
        {
            get;
        }

        IReadOnlyList<CropPlot> Plots
        {
            get;
        }

        void ClearSelection();
        void Effect(GameObject prefab, Vector3 position);
        void Save();
    }
}
