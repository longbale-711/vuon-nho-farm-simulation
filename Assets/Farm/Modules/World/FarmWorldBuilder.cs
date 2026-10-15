using System.Collections.Generic;
using Orchard.Bootstrap;
using Orchard.Crops;
using Orchard.Navigation;
using UnityEngine;

namespace Orchard.World
{
    public sealed class FarmWorldBuilder
    {
        private readonly Transform _parent;
        private readonly ICropContext _context;
        private readonly FarmResources _resources;
        private readonly FarmNavigation _navigation;
        private readonly ICollection<CropPlot> _plots;
        private readonly WorldTextFactory _labels;
        public Camera Camera
        {
            get; private set;
        }

        public FarmWorldBuilder(Transform parent, ICropContext context, FarmResources resources, FarmNavigation navigation, ICollection<CropPlot> plots, WorldTextFactory labels)
        {
            _parent = parent;
            _context = context;
            _resources = resources;
            _navigation = navigation;
            _plots = plots;
            _labels = labels;
        }

        public void Build()
        {
            CreateCamera();
            ConfigureLighting();
            CreateGround();
            CreatePlots();
            CreateMarket();
            CreateFences();
            CreateMarketAwning();
            CreateRockObstacle();
        }

        private void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            Camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            Camera.orthographic = true;
            Camera.orthographicSize = 14;
            Camera.backgroundColor = new Color32(203, 227, 207, 255);
            Camera.clearFlags = CameraClearFlags.SolidColor;
            Camera.farClipPlane = 80;
            cameraObject.transform.position = new Vector3(0, 23, -11.5f);
            cameraObject.transform.LookAt(new Vector3(0, 0, 3));
            cameraObject.AddComponent<AudioListener>();
            _labels.Initialize(Camera, _resources.Font);
        }

        private void ConfigureLighting()
        {
            var light = new GameObject("Sun").AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            light.color = new Color(1, 0.95f, 0.84f);
            light.shadows = LightShadows.Soft;
            light.shadowStrength = 0.28f;
            light.shadowBias = 0.06f;
            light.transform.rotation = Quaternion.Euler(48, -32, 0);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.7f, 0.78f, 0.69f);
        }

        private void CreateGround()
        {
            FarmVisual.Primitive("Farm island", PrimitiveType.Cube, _parent, new Vector3(0, -0.32f, 3), new Vector3(14.8f, 0.55f, 21.5f), new Color32(160, 203, 116, 255));
            FarmVisual.Primitive("Market path", PrimitiveType.Cube, _parent, new Vector3(0, -0.015f, 9.8f), new Vector3(13.5f, 0.05f, 7.2f), new Color32(231, 218, 183, 255));
            FarmVisual.Primitive("Center path", PrimitiveType.Cube, _parent, new Vector3(0, -0.012f, -1), new Vector3(2, 0.055f, 11), new Color32(222, 207, 165, 255));
        }

        private void CreatePlots()
        {
            for (int row = 0; row < 3; row++)
            {
                FarmVisual.Primitive("Cross path", PrimitiveType.Cube, _parent, new Vector3(0, -0.01f, -5.6f + row * 3.4f), new Vector3(12, 0.055f, 0.8f), new Color32(222, 207, 165, 255));
                for (int col = 0; col < 2; col++)
                {
                    var instance = new GameObject("Plot " + (row * 2 + col + 1));
                    instance.transform.SetParent(_parent);
                    instance.transform.position = new Vector3(col == 0 ? -2.8f : 2.8f, 0, -4 + row * 3.4f);
                    var plot = instance.AddComponent<CropPlot>();
                    _plots.Add(plot);
                    _navigation.AddObstacle(instance.transform.position, new Vector2(1.8f, 1.8f));
                    plot.Initialize(_context, row * 2 + col);
                }
            }
        }

        private void CreateMarket()
        {
            var market = Object.Instantiate(_resources.MarketPrefab, new Vector3(0, 0, 6.6f), Quaternion.identity, _parent);
            foreach (Transform node in market.GetComponentsInChildren<Transform>(true))
            {
                if (node.name == "Zone")
                {
                    node.gameObject.SetActive(false);
                }
            }

            _navigation.AddObstacle(new Vector3(0, 0, 6.6f), new Vector2(10, 1));
        }

        private void CreateFences()
        {
            for (int i = 0; i < 18; i++)
            {
                float z = -6.7f + i * 1.12f;
                for (int side = -1; side <= 1; side += 2)
                {
                    FarmVisual.Primitive("Fence post", PrimitiveType.Cube, _parent, new Vector3(side * 7, 0.35f, z), new Vector3(0.15f, 0.8f, 0.15f), FarmVisual.Cream);
                    FarmVisual.Primitive("Fence rail", PrimitiveType.Cube, _parent, new Vector3(side * 7, 0.43f, z + 0.55f), new Vector3(0.09f, 0.15f, 1.12f), FarmVisual.Cream);
                }
            }
        }

        private void CreateMarketAwning()
        {
            for (int lane = 0; lane < 3; lane++)
            {
                _labels.Create(_parent, new Vector3((lane - 1) * 3, 1.15f, 6.5f), "QUẦY " + (lane + 1), 2.2f, 0.6f, 3);
                FarmVisual.Primitive("Awning", PrimitiveType.Cube, _parent, new Vector3((lane - 1) * 3, 2.5f, 6.6f), new Vector3(2.9f, 0.18f, 1.5f), lane % 2 == 0 ? FarmVisual.Forest : FarmVisual.Cream);
                for (int side = -1; side <= 1; side += 2)
                {
                    FarmVisual.Primitive("Awning post", PrimitiveType.Cube, _parent, new Vector3((lane - 1) * 3 + side * 1.3f, 1.3f, 7.2f), new Vector3(0.1f, 2.5f, 0.1f), FarmVisual.Wood);
                }
            }
        }

        private void CreateRockObstacle()
        {
            var rock = new Vector3(0, 0, 3.8f);
            FarmVisual.Primitive("Rock bed", PrimitiveType.Cylinder, _parent, rock + Vector3.up * 0.2f, new Vector3(1.15f, 0.3f, 1.15f), new Color32(121, 144, 126, 255));
            _navigation.AddObstacle(rock, new Vector2(1.15f, 1.15f));
        }
    }
}
