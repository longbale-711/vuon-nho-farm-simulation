using System;
using Orchard.Bootstrap;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Orchard.Editor
{
    internal static class FarmSceneBuilder
    {
        public const string ScenePath = "Assets/Scenes/Farm.unity";
        public static void Create()
        {
            FarmFontBuilder.EnsureFont();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = new GameObject("Orchard · Farm Simulation");
            var game = root.AddComponent<FarmGame>();
            var so = new SerializedObject(game);
            Assign(so, "BoxPrefab", "Assets/Prefabs/Box.prefab");
            Assign(so, "CropPrefab", "Assets/Prefabs/Construction.prefab");
            Assign(so, "WorkerPrefab", "Assets/Prefabs/Delivery.prefab");
            Assign(so, "CustomerPrefab", "Assets/Prefabs/Customer.prefab");
            Assign(so, "MarketPrefab", "Assets/Prefabs/Market.prefab");
            Assign(so, "BuildEffect", "Assets/Prefabs/Effects/EffBuildDone.prefab");
            Assign(so, "PayEffect", "Assets/Prefabs/Effects/EffPay.prefab");
            Assign(so, "Font", "Assets/Farm/Modules/Presentation/Assets/OrchardFont.asset");
            Assign(so, "RoundedSprite", "Assets/Sprites/Common/Rounded.png");
            Assign(so, "CoinSprite", "Assets/Sprites/Currency/Cash.png");
            Assign(so, "CustomerSprite", "Assets/Sprites/Upgrade/Customer.png");
            Assign(so, "ProfitSprite", "Assets/Sprites/Upgrade/GlobalProfit.png");
            Assign(so, "StaffSprite", "Assets/Sprites/Upgrade/Staff.png");
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
        }

        private static void Assign(SerializedObject so, string name, string path)
        {
            UnityEngine.Object asset = name.EndsWith("Sprite", StringComparison.Ordinal) ? AssetDatabase.LoadAssetAtPath<Sprite>(path) : AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            if (!asset && name.EndsWith("Sprite", StringComparison.Ordinal))
            {
                foreach (var child in AssetDatabase.LoadAllAssetsAtPath(path))
                {
                    if (child is Sprite sprite && sprite.name == "Rounded")
                    {
                        asset = sprite;
                        break;
                    }
                }
            }

            if (!asset)
            {
                throw new InvalidOperationException("Missing required asset " + path);
            }

            so.FindProperty("<" + name + ">k__BackingField").objectReferenceValue = asset;
        }
    }
}
