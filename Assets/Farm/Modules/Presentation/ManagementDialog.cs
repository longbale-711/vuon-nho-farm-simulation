using System;
using System.Collections.Generic;
using Orchard.Bootstrap;
using Orchard.Currency;
using Orchard.Management;
using Orchard.World;
using TMPro;
using UnityEngine;

namespace Orchard.Presentation
{
    public sealed class ManagementDialog : IFarmDialog
    {
        private readonly IFarmUiContext _context;
        private readonly FarmUiFactory _factory;
        private readonly FarmModalHost _host;
        private readonly Action<string> _notify;
        private readonly List<Action> _rowRefresh = new List<Action>();
        private RectTransform _card;
        public ManagementDialog(IFarmUiContext context, FarmUiFactory factory, FarmModalHost host, Action<string> notify)
        {
            _context = context;
            _factory = factory;
            _host = host;
            _notify = notify;
        }

        public void Refresh()
        {
            foreach (Action refresh in _rowRefresh)
            {
                refresh();
            }
        }

        public void OpenManagement()
        {
            _context.ClearSelection();
            _rowRefresh.Clear();
            RectTransform card = _host.Show("QUẢN LÝ NÔNG TRẠI", 1420);
            _card = card;
            _factory.CreateLabel("Management subtitle", card, new Vector2(0, 518), new Vector2(830, 95), "Đầu tư để mỗi chuyến hàng có giá trị hơn.\nCác nguồn lợi nhuận nhân với nhau.", 27, FarmVisual.Ink);
            AddUpgrade(ManagementUpgrade.FirstCropProfit, "Cây số 1", "×2 lợi nhuận riêng cây đầu tiên", _context.Resources.ProfitSprite, 353);
            AddUpgrade(ManagementUpgrade.GlobalProfit, "Toàn nông trại", "×2 lợi nhuận tất cả cây trồng", _context.Resources.ProfitSprite, 146);
            AddUpgrade(ManagementUpgrade.Customers, "Thêm khách hàng", "+2 khách tới xếp hàng mua hoa quả", _context.Resources.CustomerSprite, -61);
            AddUpgrade(ManagementUpgrade.WorkerSpeed, "Bước chân nhanh", "+20% tốc độ di chuyển của dân", _context.Resources.StaffSprite, -268);
            AddUpgrade(ManagementUpgrade.ExtraWorker, "Thêm dân giao hàng", "+1 dân thu hoạch và giao hàng", _context.Resources.StaffSprite, -475);
            _factory.CreateLabel("Saved", card, new Vector2(0, -645), new Vector2(830, 60), "Tiến độ được lưu tự động trên thiết bị", 25, FarmVisual.Forest);
            Refresh();
        }

        private void AddUpgrade(ManagementUpgrade type, string title, string detail, Sprite icon, float y)
        {
            var row = _factory.CreatePanel(type.ToString(), _card, new Vector2(0, y), new Vector2(855, 183), new Vector2(0.5f, 0.5f), Color.white);
            _factory.CreateIcon(row, icon, new Vector2(-348, 0), new Vector2(87, 87));
            _factory.CreateLabel("Name", row, new Vector2(-70, 45), new Vector2(420, 52), title, 33, FarmVisual.Ink, TextAlignmentOptions.Left);
            var description = _factory.CreateLabel("Description", row, new Vector2(-70, -21), new Vector2(420, 85), detail, 25, FarmVisual.Ink, TextAlignmentOptions.Left);
            var button = _factory.CreateButton("Buy", row, new Vector2(282, 0), new Vector2(248, 115), "", FarmVisual.Forest, () =>
            {
                if (_context.Economy.Purchase(type))
                {
                    _notify("Đã nâng cấp: " + title);
                    _context.Save();
                    Refresh();
                }
            }, 30);
            var text = button.GetComponentInChildren<TMP_Text>();
            _rowRefresh.Add(() =>
            {
                bool available = _context.Economy.ManagementAvailable(type);
                Money cost = _context.Economy.ManagementCost(type);
                button.interactable = available && _context.Economy.Balance >= cost;
                text.text = !available ? (type == ManagementUpgrade.FirstCropProfit && !_context.Economy.Crops[0].Built ? "XÂY CÂY 1\nTRƯỚC" : "TỐI ĐA") : cost.Display() + " $\n<size=23>NÂNG CẤP</size>";
                description.text = detail + "\n<size=22>Cấp nâng cấp: " + _context.Economy.ManagementLevel(type) + "</size>";
            });
        }
    }
}
