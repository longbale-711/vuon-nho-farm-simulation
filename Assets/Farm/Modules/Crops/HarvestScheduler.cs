using System.Collections.Generic;
using Orchard.Navigation;
using UnityEngine;

namespace Orchard.Crops
{
    public sealed class HarvestScheduler
    {
        private readonly IReadOnlyList<CropPlot> _plots;
        private readonly FarmNavigation _navigation;
        public HarvestScheduler(IReadOnlyList<CropPlot> plots, FarmNavigation navigation)
        {
            _plots = plots;
            _navigation = navigation;
        }

        public CropPlot ReserveNearestCrop(Vector3 position)
        {
            CropPlot nearest = null;
            float shortestDistance = float.MaxValue;
            foreach (CropPlot plot in _plots)
            {
                if (plot.State != CropState.Ready)
                {
                    continue;
                }

                List<Vector3> path = _navigation.Find(position, plot.HarvestPoint);
                if (path == null)
                {
                    continue;
                }

                float distance = GetPathLength(position, path);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearest = plot;
                }
            }

            return nearest != null && nearest.TryReserve() ? nearest : null;
        }

        private static float GetPathLength(Vector3 origin, IEnumerable<Vector3> path)
        {
            float distance = 0f;
            Vector3 previous = origin;
            foreach (Vector3 point in path)
            {
                distance += Vector3.Distance(previous, point);
                previous = point;
            }

            return distance;
        }
    }
}
