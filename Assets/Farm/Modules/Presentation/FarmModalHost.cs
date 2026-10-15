using Orchard.World;
using TMPro;
using UnityEngine;

namespace Orchard.Presentation
{
    public sealed class FarmModalHost
    {
        private readonly FarmUiFactory _factory;
        private readonly UnityEngine.Events.UnityAction _close;
        private readonly RectTransform _modal;
        private RectTransform _card;
        public TMP_Text Title
        {
            get; private set;
        }
        public bool IsOpen => _modal.gameObject.activeSelf;

        public FarmModalHost(Transform parent, FarmUiFactory factory, UnityEngine.Events.UnityAction close)
        {
            _factory = factory;
            _close = close;
            _modal = factory.CreateRect("Modal", parent, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));
            _modal.anchorMin = Vector2.zero;
            _modal.anchorMax = Vector2.one;
            _modal.offsetMin = _modal.offsetMax = Vector2.zero;
            var image = _modal.gameObject.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0.05f, 0.15f, 0.1f, 0.58f);
            var dismiss = _modal.gameObject.AddComponent<UnityEngine.UI.Button>();
            dismiss.onClick.AddListener(close);
            Close();
        }

        public void Close()
        {
            _modal.gameObject.SetActive(false);
        }

        public RectTransform Show(string title, float height)
        {
            if (_card)
            {
                _card.gameObject.SetActive(false);
                Object.Destroy(_card.gameObject);
            }

            _card = _factory.CreatePanel("Card", _modal, Vector2.zero, new Vector2(970, height), new Vector2(0.5f, 0.5f), FarmVisual.Cream);
            _card.GetComponent<UnityEngine.UI.Image>().raycastTarget = true;
            Title = _factory.CreateLabel("Title", _card, new Vector2(-40, height / 2 - 88), new Vector2(790, 90), title, 43, FarmVisual.Ink);
            _factory.CreateButton("Close", _card, new Vector2(392, height / 2 - 88), new Vector2(85, 85), "×", new Color32(223, 231, 209, 255), _close, 48);
            _modal.gameObject.SetActive(true);
            return _card;
        }
    }
}
