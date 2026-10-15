using Orchard.Crops;
using Orchard.Economy;
using TMPro;
using UnityEngine;

namespace Orchard.Bootstrap
{
    public interface ICropContext
    {
        FarmEconomy Economy
        {
            get;
        }

        FarmResources Resources
        {
            get;
        }

        CropPlot SelectedPlot
        {
            get;
        }

        TMP_Text WorldLabel(Transform parent, Vector3 position, string value, float width, float height, float size);
        void Effect(GameObject prefab, Vector3 position);
    }
}
