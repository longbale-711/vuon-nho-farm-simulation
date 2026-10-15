using Orchard.Bootstrap;
using Orchard.Movement;
using UnityEngine;

namespace Orchard.Market
{
    public sealed class CustomerAgent : MonoBehaviour
    {
        public int Lane
        {
            get; private set;
        }
        public CustomerState State
        {
            get; private set;
        }
        public long ArrivalOrder
        {
            get; private set;
        }
        public bool CanServe => State == CustomerState.Waiting && _queueRank == 0;

        private ICustomerContext _context;
        private ActorMover _mover;
        private int _queueRank;
        private float _timer;
        private Vector3 _queuePoint;
        public void Initialize(ICustomerContext owner, int lane, int rank)
        {
            _context = owner;
            Lane = lane;
            _queueRank = rank;
            _mover = gameObject.AddComponent<ActorMover>();
            _mover.Initialize();
            ArrivalOrder = _context.NextArrivalOrder();
            State = CustomerState.Arriving;
            SetQueueRank(rank);
        }

        public void SetQueueRank(int rank)
        {
            _queueRank = rank;
            _queuePoint = new Vector3((Lane - 1) * 3, 0, 8.2f + rank * 1.1f);
            if (State == CustomerState.Arriving || State == CustomerState.Waiting)
            {
                State = CustomerState.Arriving;
                if (!_mover.Go(_context.Navigation, _queuePoint))
                {
                    State = CustomerState.Returning;
                    _timer = 1;
                }
            }
        }

        public bool Reserve()
        {
            if (!CanServe)
            {
                return false;
            }

            State = CustomerState.Reserved;
            return true;
        }

        public void Release()
        {
            if (State == CustomerState.Reserved)
            {
                State = CustomerState.Waiting;
            }
        }

        public void Pay()
        {
            State = CustomerState.Paying;
            _timer = 0.55f;
        }

        public void Tick(float deltaTime)
        {
            _mover.Tick(deltaTime, 2.8f);
            _timer -= deltaTime;
            switch (State)
            {
                case CustomerState.Arriving:
                    UpdateArriving();
                    break;
                case CustomerState.Paying:
                    UpdatePaying();
                    break;
                case CustomerState.Leaving:
                    UpdateLeaving();
                    break;
                case CustomerState.Returning:
                    UpdateReturning();
                    break;
            }
        }

        private void UpdateArriving()
        {
            if (_mover.Arrived)
            {
                State = CustomerState.Waiting;
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }

        private void UpdatePaying()
        {
            if (_timer <= 0)
            {
                State = CustomerState.Leaving;
                _mover.Go(_context.Navigation, new Vector3(-6.8f, 0, 12.4f));
                _context.ReorderQueue(Lane);
            }
        }

        private void UpdateLeaving()
        {
            if (_mover.Arrived)
            {
                State = CustomerState.Returning;
                _timer = 1.1f;
            }
        }

        private void UpdateReturning()
        {
            if (_timer <= 0)
            {
                transform.position = new Vector3(6.8f, 0, 12.4f);
                ArrivalOrder = _context.NextArrivalOrder();
                State = CustomerState.Arriving;
                _context.ReorderQueue(Lane, this);
            }
        }
    }
}
