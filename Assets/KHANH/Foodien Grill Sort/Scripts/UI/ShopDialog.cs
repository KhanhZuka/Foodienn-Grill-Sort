using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KHANH.FoodienGrillSort
{
    public class ShopDialog : Dialog
    {
        [SerializeField] private Text coinTxt;
        [SerializeField] private List<GiftPack> _giftPacks;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            for (int i = 0; i < _giftPacks.Count; i++)
            {
                GiftPack gift = _giftPacks[i];

                gift.BuyBtn.onClick.AddListener(() =>
                {
                    BuyGift(gift);
                });
            }
        }

        public void BuyGift(GiftPack gift)
        {
            if(Pref.Coin >= gift.Price)
            {
                Pref.Magnet += gift.MagnetAmount;
                Pref.Shuffle += gift.ShuffleAmount;
                Pref.ExtraGrill += gift.ExtraGrillAmount;
                Pref.Heart += gift.HeartAmount;
                Pref.Coin -= gift.Price;
                coinTxt.text = Pref.Coin.ToString();
                GUIManager.Instance.UpdateHomeCoin();
                GUIManager.Instance.UpdateHomeHeart();
            }
            else
            {
                Debug.Log("ban ko du xu");
            }
        }

        public override void Show(bool isShow)
        {
            base.Show(isShow);
            coinTxt.text = Pref.Coin.ToString();
        }

        public override void Close()
        {
            base.Close();
        }
    }
}

