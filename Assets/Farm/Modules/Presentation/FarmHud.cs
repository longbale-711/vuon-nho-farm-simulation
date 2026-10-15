using Orchard.Bootstrap;
using Orchard.Crops;
using Orchard.World;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Orchard.Presentation
{
    public sealed class FarmHud : MonoBehaviour
    {
        private IFarmUiContext _context;
        private RectTransform _safe;
        private TMP_Text _balance;
        private TMP_Text _summary;
        private TMP_Text _hint;
        private TMP_Text _toast;
        private float _refreshTime;
        private float _toastTime;
        private Rect _lastSafeArea;
        private FarmUiFactory _factory;
        private FarmModalHost _modal;
        private CropDialog _cropDialog;
        private ManagementDialog _managementDialog;
        private IFarmDialog _activeDialog;
        public bool ModalOpen => _modal != null && _modal.IsOpen;

        public void Initialize(IFarmUiContext owner)
        {
            _context = owner;
            _factory = new FarmUiFactory(_context.Resources);
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            if (!EventSystem.current)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }

            _safe = _factory.CreateRect("Safe area", transform, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));
            ApplySafeArea();
            var header = _factory.CreatePanel("Header", _safe, new Vector2(0, -145), new Vector2(1000, 230), new Vector2(0.5f, 1), FarmVisual.Cream);
            _factory.CreateLabel("Brand", header, new Vector2(-270, 47), new Vector2(400, 65), "VƯỜN NHỎ", 44, FarmVisual.Forest, TextAlignmentOptions.Left);
            _factory.CreateLabel("Subtitle", header, new Vector2(-270, -9), new Vector2(400, 42), "NÔNG TRẠI HOA QUẢ", 22, FarmVisual.Ink, TextAlignmentOptions.Left);
            _factory.CreateIcon(header, _context.Resources.CoinSprite, new Vector2(100, 30), new Vector2(62, 62));
            _balance = _factory.CreateLabel("Money", header, new Vector2(290, 30), new Vector2(310, 70), "", 49, FarmVisual.Ink, TextAlignmentOptions.Right);
            _summary = _factory.CreateLabel("Summary", header, new Vector2(0, -77), new Vector2(890, 43), "", 25, FarmVisual.Ink);
            var footer = _factory.CreatePanel("Footer", _safe, new Vector2(0, 138), new Vector2(1000, 200), new Vector2(0.5f, 0), FarmVisual.Cream);
            _factory.CreateButton("Management", footer, new Vector2(210, 0), new Vector2(475, 125), "QUẢN LÝ", FarmVisual.Forest, OpenManagement, 35);
            _factory.CreateLabel("Progress label", footer, new Vector2(-272, 29), new Vector2(370, 40), "TỪ VƯỜN ĐẾN QUẦY", 22, FarmVisual.Forest, TextAlignmentOptions.Left);
            _factory.CreateLabel("Progress detail", footer, new Vector2(-272, -28), new Vector2(370, 65), "Thu hoạch · Giao hàng\nTự động nhận tiền", 25, FarmVisual.Ink, TextAlignmentOptions.Left);
            _hint = _factory.CreateLabel("Hint", _safe, new Vector2(0, 285), new Vector2(970, 90), "", 29, FarmVisual.Ink, anchor: new Vector2(0.5f, 0));
            _toast = _factory.CreateLabel("Toast", _safe, new Vector2(0, -350), new Vector2(950, 70), "", 30, FarmVisual.Forest, anchor: new Vector2(0.5f, 1));
            _modal = new FarmModalHost(transform, _factory, CloseModal);
            _cropDialog = new CropDialog(_context, _factory, _modal, Notify, CloseModal);
            _managementDialog = new ManagementDialog(_context, _factory, _modal, Notify);
            Refresh();
        }

        private void ApplySafeArea()
        {
            _lastSafeArea = Screen.safeArea;
            _safe.anchorMin = new Vector2(_lastSafeArea.xMin / Screen.width, _lastSafeArea.yMin / Screen.height);
            _safe.anchorMax = new Vector2(_lastSafeArea.xMax / Screen.width, _lastSafeArea.yMax / Screen.height);
            _safe.offsetMin = _safe.offsetMax = Vector2.zero;
        }

        private void Update()
        {
            if (_lastSafeArea != Screen.safeArea)
            {
                ApplySafeArea();
            }

            _refreshTime += Time.unscaledDeltaTime;
            if (_refreshTime >= 0.25f)
            {
                _refreshTime = 0;
                Refresh();
            }

            if (_toastTime > 0)
            {
                _toastTime -= Time.unscaledDeltaTime;
                if (_toastTime <= 0)
                {
                    _toast.text = "";
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) && ModalOpen)
            {
                CloseModal();
            }
        }

        public void Refresh()
        {
            if (!_balance)
            {
                return;
            }

            var economy = _context.Economy;
            _balance.text = economy.Balance.Display() + " $";
            int built = 0;
            foreach (var crop in economy.Crops)
            {
                if (crop.Built)
                {
                    built++;
                }
            }

            _summary.text = built + "/6 cây  ·  " + economy.WorkerCount + " dân  ·  " + economy.CustomerCount + " khách  ·  " + economy.Sales + " đơn đã bán";
            _hint.text = built == 0 ? "Chạm vào hộp để xây cây đầu tiên\nBạn có 150 $ để bắt đầu" : "Chạm cây để nâng cấp lợi nhuận\nDân tự thu hoạch và giao hàng cho khách";
            if (ModalOpen)
            {
                _activeDialog?.Refresh();
            }
        }

        public void OpenPlot(CropPlot plot)
        {
            _activeDialog = _cropDialog;
            _cropDialog.OpenPlot(plot);
        }

        public void OpenManagement()
        {
            _activeDialog = _managementDialog;
            _managementDialog.OpenManagement();
        }

        public void CloseModal()
        {
            _modal.Close();
            _activeDialog = null;
            _context.ClearSelection();
        }

        private void Notify(string message)
        {
            _toast.text = message;
            _toastTime = 2.5f;
        }
    }
}
