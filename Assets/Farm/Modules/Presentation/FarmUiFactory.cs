using Orchard.Bootstrap;
using TMPro;
using UnityEngine;

namespace Orchard.Presentation
{
    public sealed class FarmUiFactory
    {
        private readonly FarmResources _resources;
        public FarmUiFactory(FarmResources resources)
        {
            _resources = resources;
        }

        public RectTransform CreateRect(string name, Transform parent, Vector2 position, Vector2 size, Vector2 anchor)
        {
            var instance = new GameObject(name, typeof(RectTransform));
            var rect = instance.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        public RectTransform CreatePanel(string name, Transform parent, Vector2 position, Vector2 size, Vector2 anchor, Color color)
        {
            var rect = CreateRect(name, parent, position, size, anchor);
            var image = rect.gameObject.AddComponent<UnityEngine.UI.Image>();
            image.sprite = _resources.RoundedSprite;
            image.type = UnityEngine.UI.Image.Type.Sliced;
            image.color = color;
            image.raycastTarget = false;
            return rect;
        }

        public TMP_Text CreateLabel(string name, Transform parent, Vector2 position, Vector2 size, string value, float fontSize, Color color, TextAlignmentOptions alignment = TextAlignmentOptions.Center, Vector2? anchor = null)
        {
            var rect = CreateRect(name, parent, position, size, anchor ?? new Vector2(0.5f, 0.5f));
            var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.font = _resources.Font;
            text.text = value;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Ellipsis;
            return text;
        }

        public void CreateIcon(Transform parent, Sprite sprite, Vector2 position, Vector2 size)
        {
            var rect = CreateRect("Icon", parent, position, size, new Vector2(0.5f, 0.5f));
            var image = rect.gameObject.AddComponent<UnityEngine.UI.Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
        }

        public UnityEngine.UI.Button CreateButton(string name, Transform parent, Vector2 position, Vector2 size, string caption, Color color, UnityEngine.Events.UnityAction action, float fontSize)
        {
            var rect = CreatePanel(name, parent, position, size, new Vector2(0.5f, 0.5f), color);
            rect.GetComponent<UnityEngine.UI.Image>().raycastTarget = true;
            var button = rect.gameObject.AddComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(action);
            var colors = button.colors;
            colors.highlightedColor = new Color(0.85f, 1, 0.88f);
            colors.pressedColor = new Color(0.7f, 0.88f, 0.73f);
            colors.disabledColor = new Color(0.56f, 0.61f, 0.55f);
            button.colors = colors;
            CreateLabel("Caption", rect, Vector2.zero, size - new Vector2(28, 12), caption, fontSize, Color.white);
            return button;
        }
    }
}
