using TMPro;
using UnityEngine;

namespace Orchard.World
{
    public sealed class WorldTextFactory
    {
        private Camera _camera;
        private TMP_FontAsset _font;
        public void Initialize(Camera camera, TMP_FontAsset font)
        {
            _camera = camera;
            _font = font;
        }

        public TMP_Text Create(Transform parent, Vector3 position, string value, float width, float height, float size)
        {
            var instance = new GameObject("World label", typeof(RectTransform));
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = position;
            instance.transform.rotation = _camera.transform.rotation;
            var text = instance.AddComponent<TextMeshPro>();
            text.font = _font;
            text.text = value;
            text.fontSize = size;
            text.alignment = TextAlignmentOptions.Center;
            text.color = FarmVisual.Ink;
            text.rectTransform.sizeDelta = new Vector2(width, height);
            text.enableWordWrapping = false;
            text.fontStyle = FontStyles.Bold;
            return text;
        }
    }
}
