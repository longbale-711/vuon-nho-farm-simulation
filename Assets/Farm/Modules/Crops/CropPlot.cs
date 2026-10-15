using System.Collections;
using Orchard.Bootstrap;
using Orchard.Currency;
using Orchard.World;
using TMPro;
using UnityEngine;

namespace Orchard.Crops
{
    public sealed class CropPlot : MonoBehaviour
    {
        public int Index
        {
            get; private set;
        }
        public CropState State
        {
            get; private set;
        }
        public Vector3 HarvestPoint => transform.position + new Vector3(0, 0, -1.6f);
        public float GrowthProgress => Mathf.Clamp01(1 - _growRemaining / GrowSeconds);

        private const float GrowSeconds = CropCatalog.GrowthSeconds;
        private ICropContext _context;
        private GameObject _box;
        private GameObject _plant;
        private TMP_Text _label;
        private float _growRemaining;
        private float _labelTimer;
        private Renderer _ring;
        public void Initialize(ICropContext owner, int index)
        {
            _context = owner;
            Index = index;
            _box = Instantiate(_context.Resources.BoxPrefab, transform);
            _box.transform.localPosition = Vector3.zero;
            _plant = Instantiate(_context.Resources.CropPrefab, transform);
            _plant.transform.localPosition = new Vector3(0, 0.5f, 0);
            foreach (Transform node in _plant.GetComponentsInChildren<Transform>(true))
            {
                if (node.name == "Box" || node.name == "BuildFocus" || node.name == "Delivery")
                {
                    node.gameObject.SetActive(false);
                }
            }

            foreach (var canvas in _plant.GetComponentsInChildren<Canvas>(true))
            {
                canvas.gameObject.SetActive(false);
            }

            var hit = gameObject.AddComponent<BoxCollider>();
            hit.center = new Vector3(0, 1, 0);
            hit.size = new Vector3(2.5f, 3, 2.5f);
            _label = _context.WorldLabel(transform, new Vector3(0, 0.18f, -1.4f), "", 2.8f, 0.55f, 2.6f);
            var selection = FarmVisual.Primitive("Selection", PrimitiveType.Cylinder, transform, new Vector3(0, -0.03f, 0), new Vector3(2.9f, 0.035f, 2.9f), FarmVisual.Mint);
            _ring = selection.GetComponent<Renderer>();
            ApplySavedState();
        }

        public void ApplySavedState()
        {
            bool built = _context.Economy.Crops[Index].Built;
            _box.SetActive(!built);
            _plant.SetActive(built);
            State = built ? CropState.Growing : CropState.Boxed;
            _growRemaining = built ? 0.5f : 0;
            RefreshLabel();
        }

        public void Tick(float deltaTime)
        {
            if (State == CropState.Growing)
            {
                _growRemaining -= deltaTime;
                if (_growRemaining <= 0)
                {
                    State = CropState.Ready;
                }
            }

            _labelTimer += deltaTime;
            if (_labelTimer >= 0.2f)
            {
                _labelTimer = 0;
                RefreshLabel();
            }

            _ring.enabled = _context.SelectedPlot == this;
        }

        public void RefreshLabel()
        {
            switch (State)
            {
                case CropState.Boxed:
                    _label.text = "Ô " + (Index + 1) + " · XÂY";
                    _label.color = FarmVisual.Ink;
                    break;
                case CropState.Building:
                    _label.text = "ĐANG XÂY...";
                    break;
                default:
                    string phase = State == CropState.Growing ? " · " + Mathf.CeilToInt(_growRemaining) + "s" : State == CropState.Reserved ? " · THU HOẠCH" : " · SẴN SÀNG";
                    _label.text = "CÂY " + (Index + 1) + " · CẤP " + _context.Economy.Crops[Index].Level + "\n" + _context.Economy.HarvestPrice(Index).Display() + " $" + phase;
                    _label.color = State == CropState.Ready ? FarmVisual.Forest : FarmVisual.Ink;
                    break;
            }
        }

        public bool Build()
        {
            if (State != CropState.Boxed || !_context.Economy.Build(Index))
            {
                return false;
            }

            StartCoroutine(OpenBox());
            return true;
        }

        private IEnumerator OpenBox()
        {
            State = CropState.Building;
            var animation = _box.GetComponentInChildren<Animation>();
            if (animation && animation.GetClip("BoxOpen"))
            {
                animation.Play("BoxOpen");
            }
            else
            {
                StartCoroutine(FarmVisual.Punch(_box.transform, 0.7f));
            }

            yield return new WaitForSeconds(0.7f);
            _box.SetActive(false);
            _plant.SetActive(true);
            State = CropState.Growing;
            _growRemaining = 0.5f;
            _context.Effect(_context.Resources.BuildEffect, transform.position + Vector3.up * 0.4f);
        }

        public bool TryReserve()
        {
            if (State != CropState.Ready)
            {
                return false;
            }

            State = CropState.Reserved;
            return true;
        }

        public void Release()
        {
            if (State == CropState.Reserved)
            {
                State = CropState.Ready;
            }
        }

        public Money Harvest()
        {
            // This immutable value travels with the worker. Later upgrades cannot reprice it.
            Money value = _context.Economy.HarvestPrice(Index);
            State = CropState.Growing;
            _growRemaining = GrowSeconds;
            StartCoroutine(FarmVisual.Punch(_plant.transform, 0.35f));
            return value;
        }
    }
}
