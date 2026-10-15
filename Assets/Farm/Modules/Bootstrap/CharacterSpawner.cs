using System.Collections.Generic;
using Orchard.Market;
using Orchard.Workers;
using UnityEngine;

namespace Orchard.Bootstrap
{
    public sealed class CharacterSpawner
    {
        private readonly FarmGame _game;
        private readonly List<WorkerAgent> _workers;
        private readonly List<CustomerAgent> _customers;
        public CharacterSpawner(FarmGame game, List<WorkerAgent> workerList, List<CustomerAgent> customerList)
        {
            _game = game;
            _workers = workerList;
            _customers = customerList;
        }

        public void Synchronize()
        {
            while (_workers.Count < _game.Economy.WorkerCount)
            {
                var instance = Object.Instantiate(_game.Resources.WorkerPrefab, new Vector3(-1 + _workers.Count * 0.65f, 0, -6), Quaternion.identity, _game.transform);
                instance.name = "Worker " + (_workers.Count + 1);
                instance.transform.localScale = Vector3.one * 0.85f;
                var worker = instance.AddComponent<WorkerAgent>();
                worker.Initialize(_game);
                _workers.Add(worker);
            }

            while (_customers.Count < _game.Economy.CustomerCount)
            {
                int index = _customers.Count;
                int lane = index % 3;
                var instance = Object.Instantiate(_game.Resources.CustomerPrefab, new Vector3(6.4f, 0, 12.4f - index * 0.1f), Quaternion.identity, _game.transform);
                instance.name = "Customer " + (index + 1);
                instance.transform.localScale = Vector3.one * 0.85f;
                var customer = instance.AddComponent<CustomerAgent>();
                customer.Initialize(_game, lane, index / 3);
                _customers.Add(customer);
                _game.ReorderQueue(lane);
            }
        }
    }
}
