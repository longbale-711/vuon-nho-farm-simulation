using Orchard.Bootstrap;
using Orchard.Crops;
using Orchard.Currency;
using Orchard.Market;
using Orchard.Movement;
using Orchard.World;
using UnityEngine;

namespace Orchard.Workers
{
    public sealed class WorkerAgent : MonoBehaviour
    {
        public WorkerState State
        {
            get; private set;
        }
        public Money CargoValue
        {
            get; private set;
        }

        private IWorkerContext _context;
        private ActorMover _mover;
        private CropPlot _crop;
        private CustomerAgent _customer;
        private GameObject _cargo;
        private float _timer;
        public void Initialize(IWorkerContext owner)
        {
            _context = owner;
            _mover = gameObject.AddComponent<ActorMover>();
            _mover.Initialize();
            _mover.Carry(false);
            foreach (Transform node in GetComponentsInChildren<Transform>(true))
            {
                if (node.name == "Tomato")
                {
                    node.gameObject.SetActive(false);
                }
            }

            _cargo = new GameObject("Harvest basket");
            _cargo.transform.SetParent(transform, false);
            FarmVisual.Primitive("Basket", PrimitiveType.Cube, _cargo.transform, new Vector3(0, 1.1f, 0.48f), new Vector3(0.7f, 0.35f, 0.52f), FarmVisual.Wood);
            for (int i = 0; i < 3; i++)
            {
                FarmVisual.Primitive("Fruit", PrimitiveType.Sphere, _cargo.transform, new Vector3((i - 1) * 0.19f, 1.32f, 0.48f), Vector3.one * 0.24f, FarmVisual.Coral);
            }

            _cargo.SetActive(false);
            State = WorkerState.Idle;
        }

        public void Tick(float deltaTime)
        {
            float speed = Mathf.Min(6, 2.5f * _context.Economy.WorkerSpeed.Multiplier.AsFloat());
            _mover.Tick(deltaTime, speed);
            _timer -= deltaTime;
            switch (State)
            {
                case WorkerState.Idle:
                    UpdateIdle();
                    break;
                case WorkerState.WalkingToCrop:
                    UpdateWalkingToCrop();
                    break;
                case WorkerState.Harvesting:
                    UpdateHarvesting();
                    break;
                case WorkerState.WaitingForCustomer:
                    UpdateWaitingForCustomer();
                    break;
                case WorkerState.Delivering:
                    UpdateDelivering();
                    break;
                case WorkerState.Selling:
                    UpdateSelling();
                    break;
            }
        }

        private void UpdateIdle()
        {
            if (_timer > 0)
            {
                return;
            }

            _timer = 0.3f;
            _crop = _context.ReserveCrop(transform.position);
            if (_crop)
            {
                if (_mover.Go(_context.Navigation, _crop.HarvestPoint))
                {
                    State = WorkerState.WalkingToCrop;
                }
                else
                {
                    _crop.Release();
                    _crop = null;
                }
            }
        }

        private void UpdateWalkingToCrop()
        {
            if (_mover.Arrived)
            {
                State = WorkerState.Harvesting;
                _timer = 0.8f;
                transform.rotation = Quaternion.LookRotation(_crop.transform.position - transform.position);
            }
        }

        private void UpdateHarvesting()
        {
            if (_timer <= 0)
            {
                CargoValue = _crop.Harvest();
                _crop = null;
                _cargo.SetActive(true);
                _mover.Carry(true);
                State = WorkerState.WaitingForCustomer;
                _timer = 0;
            }
        }

        private void UpdateWaitingForCustomer()
        {
            if (_timer > 0)
            {
                return;
            }

            _timer = 0.25f;
            _customer = _context.ReserveCustomer();
            if (_customer)
            {
                if (_mover.Go(_context.Navigation, new Vector3((_customer.Lane - 1) * 3, 0, 5.15f)))
                {
                    State = WorkerState.Delivering;
                }
                else
                {
                    _customer.Release();
                    _customer = null;
                }
            }
        }

        private void UpdateDelivering()
        {
            if (_mover.Arrived)
            {
                State = WorkerState.Selling;
                _timer = 0.45f;
                transform.rotation = Quaternion.identity;
            }
        }

        private void UpdateSelling()
        {
            if (_timer <= 0)
            {
                var payment = CargoValue;
                CargoValue = Money.Coins(0);
                _customer.Pay();
                _customer = null;
                State = WorkerState.Idle;
                _cargo.SetActive(false);
                _mover.Carry(false);
                _context.Economy.Sell(payment);
                _context.ShowPayment(transform.position + Vector3.up * 2, payment);
                _context.Effect(_context.Resources.PayEffect, transform.position + Vector3.up * 1.2f);
            }
        }

        private void OnDestroy()
        {
            if (_crop)
            {
                _crop.Release();
            }

            if (_customer)
            {
                _customer.Release();
            }
        }
    }
}
