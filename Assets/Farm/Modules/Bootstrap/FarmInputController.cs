using System;
using Orchard.Crops;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Orchard.Bootstrap
{
    public sealed class FarmInputController
    {
        private readonly Camera _camera;
        private readonly Func<bool> _isModalOpen;
        private readonly Action<CropPlot> _selectPlot;
        private float _lastInputTime;
        public FarmInputController(Camera camera, Func<bool> isModalOpen, Action<CropPlot> selectPlot)
        {
            _camera = camera;
            _isModalOpen = isModalOpen;
            _selectPlot = selectPlot;
        }

        public void Tick()
        {
            if (!Input.GetMouseButtonDown(0) || Time.unscaledTime - _lastInputTime <= 0.1f || _isModalOpen())
            {
                return;
            }

            _lastInputTime = Time.unscaledTime;
            if (IsPointerOverUi())
            {
                return;
            }

            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f))
            {
                CropPlot plot = hit.collider.GetComponent<CropPlot>();
                if (plot != null)
                {
                    _selectPlot(plot);
                }
            }
        }

        private static bool IsPointerOverUi()
        {
            if (EventSystem.current == null)
            {
                return false;
            }

            return Input.touchCount > 0 ? EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) : EventSystem.current.IsPointerOverGameObject();
        }
    }
}
