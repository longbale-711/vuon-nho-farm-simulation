using System;
using Orchard.Bootstrap;
using Orchard.Crops;
using Orchard.Currency;
using Orchard.World;
using TMPro;
using UnityEngine;

namespace Orchard.Presentation
{
    public sealed class CropDialog : IFarmDialog
    {
        private readonly IFarmUiContext _context;
        private readonly FarmUiFactory _factory;
        private readonly FarmModalHost _host;
        private readonly Action<string> _notify;
        private readonly Action _close;
        private CropPlot _currentPlot;
        private TMP_Text _modalInfo;
        private TMP_Text _purchaseText;
        private UnityEngine.UI.Button _purchase;
        public CropDialog(IFarmUiContext context, FarmUiFactory factory, FarmModalHost host, Action<string> notify, Action close)
        {
            _context = context;
            _factory = factory;
            _host = host;
            _notify = notify;
            _close = close;
        }

        public void OpenPlot(CropPlot plot)
        {
            RectTransform card = _host.Show("", 1030);
            _currentPlot = plot;
            var icon = _context.Economy.Crops[plot.Index].Built ? _context.Resources.ProfitSprite : _context.Resources.CoinSprite;
            _factory.CreateIcon(card, icon, new Vector2(0, 263), new Vector2(135, 135));
            _modalInfo = _factory.CreateLabel("Info", card, new Vector2(0, 34), new Vector2(820, 350), "", 34, FarmVisual.Ink);
            _purchase = _factory.CreateButton("Purchase", card, new Vector2(0, -265), new Vector2(800, 145), "", FarmVisual.Forest, PurchasePlot, 36);
            _purchaseText = _purchase.GetComponentInChildren<TMP_Text>();
            _factory.CreateLabel("Harvest pricing", card, new Vector2(0, -401), new Vector2(820, 95), "Giá bán được chốt khi dân thu hoạch.\nNâng cấp sau đó áp dụng cho lượt tiếp theo.", 25, FarmVisual.Ink);
            Refresh();
        }

        public void Refresh()
        {
            int index = _currentPlot.Index;
            CropAccount crop = _context.Economy.Crops[index];
            Money cost = crop.Built ? _context.Economy.UpgradeCost(index) : _context.Economy.BuildCost(index);
            bool isAtMaximumLevel = crop.Level >= CropCatalog.MaximumLevel;
            _host.Title.text = crop.Built ? $"CÂY {index + 1} · CẤP {crop.Level}" : $"XÂY CÂY {index + 1}";
            _modalInfo.text = GetDescription(index, crop, isAtMaximumLevel);
            RefreshPurchaseButton(crop.Built, isAtMaximumLevel, cost);
        }

        private string GetDescription(int index, CropAccount crop, bool isAtMaximumLevel)
        {
            if (!crop.Built)
            {
                return "MỞ THÊM MỘT Ô TRỒNG\n\nDân sẽ tự đến thu hoạch\nvà mang hoa quả ra quầy bán.\n\nChu kỳ mọc quả: 3,5 giây";
            }

            string currentPrice = _context.Economy.HarvestPrice(index).Display();
            string nextLevel = isAtMaximumLevel ? "Đã đạt cấp tối đa" : $"Cấp {crop.Level + 1}  →  {_context.Economy.NextHarvestPrice(index).Display()} $";
            return $"MỖI LẦN THU HOẠCH\n<size=58><b>{currentPrice} $</b></size>\n\n{nextLevel}\nTăng thêm 10% giá gốc mỗi cấp";
        }

        private void RefreshPurchaseButton(bool isBuilt, bool isAtMaximumLevel, Money cost)
        {
            bool isBuilding = _currentPlot.State == CropState.Building;
            bool canAfford = _context.Economy.Balance >= cost;
            _purchase.interactable = !isBuilding && !isAtMaximumLevel && canAfford;
            if (isBuilding)
            {
                _purchaseText.text = "ĐANG MỞ HỘP...";
                return;
            }

            if (isAtMaximumLevel)
            {
                _purchaseText.text = "ĐÃ ĐẠT CẤP TỐI ĐA";
                return;
            }

            string action = isBuilt ? "NÂNG CẤP" : "XÂY CÂY";
            string affordability = canAfford ? string.Empty : "\n<size=25>Chưa đủ tiền</size>";
            _purchaseText.text = $"{action}  ·  {cost.Display()} ${affordability}";
        }

        private void PurchasePlot()
        {
            if (!_currentPlot)
            {
                return;
            }

            bool built = _context.Economy.Crops[_currentPlot.Index].Built;
            if (built ? _context.Economy.Upgrade(_currentPlot.Index) : _currentPlot.Build())
            {
                _notify(built ? "Đã tăng lợi nhuận cây!" : "Đang mở hộp trồng cây...");
                if (built)
                {
                    _context.Effect(_context.Resources.BuildEffect, _currentPlot.transform.position + Vector3.up);
                }

                _context.Save();
                if (!built)
                {
                    _close();
                }
                else
                {
                    Refresh();
                }
            }
        }
    }
}
