using System.Collections.Generic;
using Orchard.Navigation;
using UnityEngine;

namespace Orchard.Movement
{
    public sealed class ActorMover : MonoBehaviour
    {
        private List<Vector3> _route;
        private int _waypoint;
        private Animator _animator;
        private bool _carrying;
        public bool Arrived => _route == null || _waypoint >= _route.Count;

        public void Initialize()
        {
            _animator = GetComponentInChildren<Animator>();
            if (_animator)
            {
                _animator.applyRootMotion = false;
            }
        }

        public bool Go(FarmNavigation navigation, Vector3 target)
        {
            var result = navigation.Find(transform.position, target);
            if (result == null)
            {
                Stop();
                return false;
            }

            _route = result;
            _waypoint = 0;
            return true;
        }

        public void Stop()
        {
            _route = null;
            Animate(false);
        }

        public void Carry(bool value)
        {
            _carrying = value;
            Animate(!Arrived);
        }

        public void Tick(float deltaTime, float speed)
        {
            if (Arrived)
            {
                Animate(false);
                return;
            }

            float remaining = speed * deltaTime;
            while (remaining > 0 && !Arrived)
            {
                Vector3 direction = _route[_waypoint] - transform.position;
                direction.y = 0;
                float distance = direction.magnitude;
                if (distance < 0.001f)
                {
                    _waypoint++;
                    continue;
                }

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Mathf.Min(1, deltaTime * 14));
                float move = Mathf.Min(distance, remaining);
                transform.position += direction / distance * move;
                remaining -= move;
                if (move >= distance)
                {
                    _waypoint++;
                }
            }

            Animate(!Arrived);
        }

        private void Animate(bool moving)
        {
            if (!_animator || !_animator.runtimeAnimatorController)
            {
                return;
            }

            _animator.SetBool("IsEmpty", !_carrying);
            _animator.SetBool("IsMove", moving && !_carrying);
            _animator.SetBool("IsCarryMove", moving && _carrying);
        }
    }
}
