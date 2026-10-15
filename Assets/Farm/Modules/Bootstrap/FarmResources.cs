using TMPro;
using UnityEngine;

namespace Orchard.Bootstrap
{
    public sealed class FarmResources
    {
        public GameObject BoxPrefab
        {
            get;
        }
        public GameObject CropPrefab
        {
            get;
        }
        public GameObject WorkerPrefab
        {
            get;
        }
        public GameObject CustomerPrefab
        {
            get;
        }
        public GameObject MarketPrefab
        {
            get;
        }
        public GameObject BuildEffect
        {
            get;
        }
        public GameObject PayEffect
        {
            get;
        }
        public TMP_FontAsset Font
        {
            get;
        }
        public Sprite RoundedSprite
        {
            get;
        }
        public Sprite CoinSprite
        {
            get;
        }
        public Sprite CustomerSprite
        {
            get;
        }
        public Sprite ProfitSprite
        {
            get;
        }
        public Sprite StaffSprite
        {
            get;
        }

        public FarmResources(FarmGame game)
        {
            BoxPrefab = game.BoxPrefab;
            CropPrefab = game.CropPrefab;
            WorkerPrefab = game.WorkerPrefab;
            CustomerPrefab = game.CustomerPrefab;
            MarketPrefab = game.MarketPrefab;
            BuildEffect = game.BuildEffect;
            PayEffect = game.PayEffect;
            Font = game.Font;
            RoundedSprite = game.RoundedSprite;
            CoinSprite = game.CoinSprite;
            CustomerSprite = game.CustomerSprite;
            ProfitSprite = game.ProfitSprite;
            StaffSprite = game.StaffSprite;
        }
    }
}
