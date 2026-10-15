using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orchard.World
{
    public static class FarmVisual
    {
        public static readonly Color Ink = new Color32(31, 65, 51, 255);
        public static readonly Color Forest = new Color32(24, 115, 71, 255);
        public static readonly Color Mint = new Color32(123, 220, 160, 255);
        public static readonly Color Cream = new Color32(250, 246, 230, 255);
        public static readonly Color Coral = new Color32(239, 106, 70, 255);
        public static readonly Color Wood = new Color32(165, 111, 64, 255);
        public static readonly Color Gold = new Color32(250, 201, 75, 255);
        private static readonly Dictionary<Color, Material> s_materials = new Dictionary<Color, Material>();
        public static Material Material(Color color)
        {
            if (s_materials.TryGetValue(color, out var found) && found)
            {
                return found;
            }

            var material = new Material(Shader.Find("Standard"))
            {
                color = color
            };
            material.SetFloat("_Glossiness", 0);
            s_materials[color] = material;
            return material;
        }

        public static GameObject Primitive(string name, PrimitiveType type, Transform parent, Vector3 position, Vector3 scale, Color color)
        {
            var instance = GameObject.CreatePrimitive(type);
            instance.name = name;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = position;
            instance.transform.localScale = scale;
            instance.GetComponent<Renderer>().sharedMaterial = Material(color);
            Object.Destroy(instance.GetComponent<Collider>());
            return instance;
        }

        public static IEnumerator Punch(Transform target, float duration)
        {
            Vector3 scale = target.localScale;
            float elapsed = 0;
            while (elapsed < duration && target)
            {
                elapsed += Time.deltaTime;
                target.localScale = scale * (1 + Mathf.Sin(elapsed / duration * Mathf.PI) * 0.16f);
                yield return null;
            }

            if (target)
            {
                target.localScale = scale;
            }
        }
    }
}
