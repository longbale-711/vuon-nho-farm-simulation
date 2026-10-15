using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace Orchard.Editor
{
    internal static class FarmFontBuilder
    {
        public static void EnsureFont()
        {
            const string FontPath = "Assets/Farm/Modules/Presentation/Assets/OrchardFont.asset";
            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath))
            {
                return;
            }

            var source = AssetDatabase.LoadAssetAtPath<Font>("Assets/TextMesh Pro/Fonts/LiberationSans.ttf");
            if (!source)
            {
                throw new InvalidOperationException("TMP Essentials font is not imported");
            }

            var font = TMP_FontAsset.CreateFontAsset(source, 90, 9, GlyphRenderMode.SDFAA, 2048, 2048, AtlasPopulationMode.Dynamic, true);
            font.name = "OrchardFont";
            string chars = "";
            for (int i = 32; i < 256; i++)
            {
                chars += (char)i;
            }

            for (int i = 0x100; i < 0x180; i++)
            {
                chars += (char)i;
            }

            for (int i = 0x1E00; i < 0x1F00; i++)
            {
                chars += (char)i;
            }

            chars += "→↗·×";
            font.TryAddCharacters(chars, out string missing);
            AssetDatabase.CreateAsset(font, FontPath);
            AssetDatabase.AddObjectToAsset(font.material, font);
            foreach (var texture in font.atlasTextures)
            {
                texture.name = "Orchard SDF Atlas";
                AssetDatabase.AddObjectToAsset(texture, font);
            }

            EditorUtility.SetDirty(font);
            AssetDatabase.SaveAssets();
        }
    }
}
