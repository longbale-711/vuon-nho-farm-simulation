using System;
using System.Collections.Generic;
using Orchard.Crops;
using Orchard.Currency;
using Orchard.Economy;
using Orchard.Market;
using Orchard.Navigation;
using Orchard.Persistence;
using Orchard.Presentation;
using Orchard.Workers;
using Orchard.World;
using TMPro;
using UnityEngine;

namespace Orchard.Bootstrap
{
    public sealed class FarmGame : MonoBehaviour, ICropContext, IWorkerContext, ICustomerContext, IFarmUiContext
    {
        [field: SerializeField]
        public GameObject BoxPrefab
        {
            get; private set;
        }

        [field: SerializeField]
        public GameObject CropPrefab
        {
            get; private set;
        }

        [field: SerializeField]
        public GameObject WorkerPrefab
        {
            get; private set;
        }

        [field: SerializeField]
        public GameObject CustomerPrefab
        {
            get; private set;
        }

        [field: SerializeField]
        public GameObject MarketPrefab
        {
            get; private set;
        }

        [field: SerializeField]
        public GameObject BuildEffect
        {
            get; private set;
        }

        [field: SerializeField]
        public GameObject PayEffect
        {
            get; private set;
        }

        [field: SerializeField]
        public TMP_FontAsset Font
        {
            get; private set;
        }

        [field: SerializeField]
        public Sprite RoundedSprite
        {
            get; private set;
        }

        [field: SerializeField]
        public Sprite CoinSprite
        {
            get; private set;
        }

        [field: SerializeField]
        public Sprite CustomerSprite
        {
            get; private set;
        }

        [field: SerializeField]
        public Sprite ProfitSprite
        {
            get; private set;
        }

        [field: SerializeField]
        public Sprite StaffSprite
        {
            get; private set;
        }

        private readonly List<CropPlot> _plots = new List<CropPlot>();
        private readonly List<WorkerAgent> _workers = new List<WorkerAgent>();
        private readonly List<CustomerAgent> _customers = new List<CustomerAgent>();
        private readonly WorldTextFactory _labels = new WorldTextFactory();
        private Camera _camera;
        private FarmEffectsPresenter _effects;
        private FarmInputController _input;
        private CharacterSpawner _spawner;
        private CustomerQueue _customerQueue;
        private HarvestScheduler _harvestScheduler;
        private ProgressPersistence _persistence;
        public FarmEconomy Economy
        {
            get; private set;
        }
        public FarmNavigation Navigation
        {
            get; private set;
        }
        public FarmResources Resources
        {
            get; private set;
        }
        public CropPlot SelectedPlot
        {
            get; private set;
        }
        public IReadOnlyList<CropPlot> Plots
        {
            get; private set;
        }
        public IReadOnlyList<WorkerAgent> Workers
        {
            get; private set;
        }
        public IReadOnlyList<CustomerAgent> Customers
        {
            get; private set;
        }
        public FarmHud Hud
        {
            get; private set;
        }

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.Portrait;
            Economy = new FarmEconomy();
            Navigation = new FarmNavigation();
            Resources = new FarmResources(this);
            Plots = _plots.AsReadOnly();
            Workers = _workers.AsReadOnly();
            Customers = _customers.AsReadOnly();
            bool isVerification = Array.Exists(Environment.GetCommandLineArgs(), argument => argument == "-farmVerify");
            var store = new SaveStore(Application.persistentDataPath);
            if (!isVerification)
            {
                store.Load(Economy);
            }

            _persistence = new ProgressPersistence(Economy, store, !isVerification);
            _customerQueue = new CustomerQueue(Customers);
            _harvestScheduler = new HarvestScheduler(Plots, Navigation);
            var world = new FarmWorldBuilder(transform, this, Resources, Navigation, _plots, _labels);
            world.Build();
            _camera = world.Camera;
            _effects = new GameObject("Farm effects").AddComponent<FarmEffectsPresenter>();
            _effects.transform.SetParent(transform, false);
            _effects.Initialize(_labels);
            Hud = new GameObject("Farm HUD").AddComponent<FarmHud>();
            Hud.Initialize(this);
            _input = new FarmInputController(_camera, () => Hud.ModalOpen, Select);
            _spawner = new CharacterSpawner(this, _workers, _customers);
            _spawner.Synchronize();
            Economy.Changed += OnEconomyChanged;
        }

        private void Update()
        {
            _camera.orthographicSize = Mathf.Max(13.4f, 7.6f / _camera.aspect);
            float deltaTime = Mathf.Min(Time.deltaTime, 0.1f);
            foreach (CropPlot plot in _plots)
            {
                plot.Tick(deltaTime);
            }

            foreach (CustomerAgent customer in _customers)
            {
                customer.Tick(deltaTime);
            }

            foreach (WorkerAgent worker in _workers)
            {
                worker.Tick(deltaTime);
            }

            _input.Tick();
            _persistence.Tick(Time.unscaledDeltaTime);
        }

        public void Select(CropPlot plot)
        {
            SelectedPlot = plot;
            Hud.OpenPlot(plot);
        }

        public void ClearSelection() => SelectedPlot = null;
        public CropPlot ReserveCrop(Vector3 position) => _harvestScheduler.ReserveNearestCrop(position);
        public CustomerAgent ReserveCustomer() => _customerQueue.ReserveCustomer();
        public long NextArrivalOrder() => _customerQueue.NextArrivalOrder();
        public void ReorderQueue(int lane, CustomerAgent returning = null) => _customerQueue.Reorder(lane, returning);
        public TMP_Text WorldLabel(Transform parent, Vector3 position, string value, float width, float height, float size) => _labels.Create(parent, position, value, width, height, size);
        public void Effect(GameObject prefab, Vector3 position) => _effects.Effect(prefab, position);
        public void ShowPayment(Vector3 position, Money payment) => _effects.ShowPayment(position, payment);
        public void Save() => _persistence.Save();
        private void OnEconomyChanged()
        {
            _persistence.MarkDirty();
            _spawner.Synchronize();
            Hud.Refresh();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                Save();
            }
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        private void OnDestroy()
        {
            if (Economy != null)
            {
                Economy.Changed -= OnEconomyChanged;
            }
        }
    }
}
