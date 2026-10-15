using Orchard.Economy;

namespace Orchard.Persistence
{
    public sealed class ProgressPersistence
    {
        private const float SaveInterval = 5f;
        private readonly FarmEconomy _economy;
        private readonly SaveStore _store;
        private readonly bool _isEnabled;
        private bool _isDirty;
        private float _elapsedTime;
        public ProgressPersistence(FarmEconomy economy, SaveStore store, bool isEnabled)
        {
            _economy = economy;
            _store = store;
            _isEnabled = isEnabled;
        }

        public void MarkDirty() => _isDirty = true;
        public void Tick(float deltaTime)
        {
            _elapsedTime += deltaTime;
            if (_elapsedTime >= SaveInterval)
            {
                _elapsedTime = 0f;
                Save();
            }
        }

        public void Save()
        {
            if (_isEnabled && _isDirty && _store.Save(_economy))
            {
                _isDirty = false;
            }
        }
    }
}
