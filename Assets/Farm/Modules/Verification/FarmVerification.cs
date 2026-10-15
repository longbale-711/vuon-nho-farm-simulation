#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.IO;
using Orchard.Bootstrap;
using Orchard.Currency;
using Orchard.Economy;
using Orchard.Management;
using Orchard.Persistence;
using UnityEngine;

namespace Orchard.Verification
{
    public sealed class FarmVerification : MonoBehaviour
    {
        private FarmGame _game;
        private string _output;
        private string _error;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (Array.Exists(Environment.GetCommandLineArgs(), arg => arg == "-farmVerify"))
            {
                new GameObject("Verification").AddComponent<FarmVerification>();
            }
        }

        private void OnEnable()
        {
            Application.logMessageReceived += RecordError;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= RecordError;
        }

        private void RecordError(string condition, string trace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            {
                _error = condition + "\n" + trace;
            }
        }

        private IEnumerator Start()
        {
            string buildFolder = Application.platform == RuntimePlatform.OSXPlayer ? Directory.GetParent(Application.dataPath).Parent.Parent.FullName : Path.GetFullPath(Path.Combine(Application.dataPath, "../Builds"));
            _output = Path.Combine(buildFolder, "Verification");
            Directory.CreateDirectory(_output);
            _game = FindObjectOfType<FarmGame>();
            if (!_game)
            {
                Finish(false, "Farm not loaded");
                yield break;
            }

            Time.timeScale = 3;
            yield return null;
            Capture("01-start");
            if (_game.Economy.CustomerCount != 1 || _game.Economy.Balance != Money.Coins(150))
            {
                Finish(false, "Invalid initial state");
                yield break;
            }

            _game.Select(_game.Plots[0]);
            yield return null;
            Capture("02-build-dialog");
            Click("Purchase");
            if (!_game.Economy.Crops[0].Built)
            {
                Finish(false, "Build button not wired");
                yield break;
            }

            int initialSales = _game.Economy.Sales;
            yield return Await(() => _game.Economy.Sales > initialSales, 30);
            if (_game.Economy.Sales == initialSales)
            {
                DumpActors();
                Finish(false, "No autonomous harvest/delivery/sale");
                yield break;
            }

            Capture("03-first-sale");
            _game.Economy.Sell(Money.Coins(100000));
            for (int i = 1; i < FarmEconomy.PlotCount; i++)
            {
                _game.Plots[i].Build();
            }

            _game.Select(_game.Plots[0]);
            Click("Purchase");
            if (_game.Economy.Crops[0].Level != 2)
            {
                Finish(false, "Upgrade button not wired");
                yield break;
            }

            Capture("04-upgrade-dialog");
            _game.Hud.CloseModal();
            foreach (ManagementUpgrade type in Enum.GetValues(typeof(ManagementUpgrade)))
            {
                if (!_game.Economy.Purchase(type))
                {
                    Finish(false, "Management purchase failed");
                    yield break;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                _game.Economy.Purchase(ManagementUpgrade.Customers);
            }

            if (_game.Customers.Count != 11 || _game.Workers.Count != 3)
            {
                Finish(false, "Actors not added after purchases");
                yield break;
            }

            initialSales = _game.Economy.Sales;
            yield return Await(() => _game.Economy.Sales >= initialSales + 15, 60);
            if (_game.Economy.Sales < initialSales + 15)
            {
                DumpActors();
                Finish(false, "Queue stalled with 11 customers");
                yield break;
            }

            Capture("05-expanded-farm");
            _game.Hud.OpenManagement();
            yield return null;
            Capture("06-management");
            var restored = new FarmEconomy();
            string saveFolder = Path.Combine(_output, "save-test");
            var store = new SaveStore(saveFolder);
            store.Save(_game.Economy);
            if (!store.Load(restored) || !restored.Balance.Equals(_game.Economy.Balance) || restored.CustomerCount != 11)
            {
                Finish(false, "Disk save roundtrip failed");
                yield break;
            }

            Finish(_error == null, _error ?? "Build/upgrade UI, 6 plots, harvest/delivery/payment, 11-customer queue, management buffs and disk save passed.");
        }

        private IEnumerator Await(Func<bool> condition, float timeout)
        {
            float deadline = Time.realtimeSinceStartup + timeout;
            while (!condition() && Time.realtimeSinceStartup < deadline)
            {
                yield return null;
            }
        }

        private void Click(string name)
        {
            foreach (var button in _game.Hud.GetComponentsInChildren<UnityEngine.UI.Button>())
            {
                if (button.name == name)
                {
                    button.onClick.Invoke();
                    return;
                }
            }

            _error = "Missing UI button " + name;
        }

        private void Capture(string name)
        {
            var cameraView = Camera.main;
            var canvas = _game.Hud.GetComponent<Canvas>();
            var renderMode = canvas.renderMode;
            var oldCamera = canvas.worldCamera;
            var texture = new RenderTexture(1080, 1920, 24);
            cameraView.targetTexture = texture;
            cameraView.aspect = 1080f / 1920;
            cameraView.orthographicSize = Mathf.Max(13.4f, 7.6f / cameraView.aspect);
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cameraView;
            canvas.planeDistance = 2;
            Canvas.ForceUpdateCanvases();
            cameraView.Render();
            var old = RenderTexture.active;
            RenderTexture.active = texture;
            var image = new Texture2D(1080, 1920, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 1080, 1920), 0, 0);
            image.Apply();
            File.WriteAllBytes(Path.Combine(_output, name + ".png"), image.EncodeToPNG());
            RenderTexture.active = old;
            cameraView.targetTexture = null;
            texture.Release();
            Destroy(texture);
            Destroy(image);
            canvas.renderMode = renderMode;
            canvas.worldCamera = oldCamera;
        }

        private void DumpActors()
        {
            string report = "";
            foreach (var worker in _game.Workers)
            {
                report += worker.name + " " + worker.State + " " + worker.transform.position + "\n";
            }

            foreach (var customer in _game.Customers)
            {
                report += customer.name + " " + customer.State + " lane " + customer.Lane + " " + customer.transform.position + "\n";
            }

            foreach (var plot in _game.Plots)
            {
                report += plot.name + " " + plot.State + "\n";
            }

            File.WriteAllText(Path.Combine(_output, "actors.txt"), report);
        }

        private void Finish(bool passed, string message)
        {
            File.WriteAllText(Path.Combine(_output, "result.txt"), (passed ? "PASS\n" : "FAIL\n") + message + "\nSales: " + (_game ? _game.Economy.Sales : 0));
            Debug.Log("[Orchard verification] " + (passed ? "PASS: " : "FAIL: ") + message);
            Application.Quit(passed ? 0 : 1);
        }
    }
}
#endif
