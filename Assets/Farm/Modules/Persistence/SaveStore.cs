using System;
using System.IO;
using Orchard.Economy;
using UnityEngine;

namespace Orchard.Persistence
{
    public sealed class SaveStore
    {
        private readonly string _path;
        public SaveStore(string folder)
        {
            _path = Path.Combine(folder, "orchard-save.json");
        }

        public bool Load(FarmEconomy economy) => TryLoad(_path, economy) || TryLoad(_path + ".bak", economy);
        private static bool TryLoad(string file, FarmEconomy economy)
        {
            try
            {
                if (!File.Exists(file))
                {
                    return false;
                }

                string json = SaveJsonMigration.Normalize(File.ReadAllText(file));
                return economy.Restore(JsonUtility.FromJson<FarmSaveData>(json)?.ToSnapshot());
            }
            catch (Exception e) when (e is IOException || e is ArgumentException || e is UnauthorizedAccessException)
            {
                Debug.LogWarning("Cannot load farm save: " + e.Message);
                return false;
            }
        }

        public bool Save(FarmEconomy economy)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_path));
                File.WriteAllText(_path + ".tmp", JsonUtility.ToJson(FarmSaveData.FromSnapshot(economy.Snapshot()), true));
                if (File.Exists(_path))
                {
                    File.Replace(_path + ".tmp", _path, _path + ".bak");
                }
                else
                {
                    File.Move(_path + ".tmp", _path);
                }

                return true;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Debug.LogWarning("Cannot save farm: " + e.Message);
                return false;
            }
        }
    }
}
