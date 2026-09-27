using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KHANH.FoodienGrillSort
{
    public class ShopDialog : Dialog
    {
        [SerializeField] private Text coinTxt;
        [SerializeField] private List<GiftPack> _giftPacks;
        [SerializeField] private Text _buyNotice;
        [SerializeField] private RectTransform _coinUI;

        private void Start()
        {
            for (int i = 0; i < _giftPacks.Count; i++)
            {
                GiftPack gift = _giftPacks[i];

                gift.BuyBtn.onClick.AddListener(() =>
                {
                    BuyGift(gift);
                });
            }

            // Ban đầu không hiện thông báo
            _buyNotice.gameObject.SetActive(false);
        }

        public void BuyGift(GiftPack gift)
        {
            Transform btnTransform = gift.BuyBtn.transform;

            if (Pref.Coin >= gift.Price)
            {
                // ===== HIỆU ỨNG MUA THÀNH CÔNG =====

                // Kill animation cũ nếu người chơi bấm liên tục
                btnTransform.DOKill();

                btnTransform.DOPunchScale(
                    Vector3.one * 0.2f,
                    0.3f,
                    5,
                    0.5f
                );

                Pref.Magnet += gift.MagnetAmount;
                Pref.Shuffle += gift.ShuffleAmount;
                Pref.ExtraGrill += gift.ExtraGrillAmount;
                Pref.Heart += gift.HeartAmount;

                Pref.Coin -= gift.Price;

                coinTxt.text = Pref.Coin.ToString();

                GUIManager.Instance.UpdateHomeCoin();
                GUIManager.Instance.UpdateHomeHeart();

                ShowNotice("Mua thành công!");
            }
            else
            {
                btnTransform.DOKill();

                btnTransform.DOShakePosition(
                    duration: 0.3f,
                    strength: 10f,
                    vibrato: 10
                );

                CoinShakeFX();

                ShowNotice("Không đủ xu!");
            }
        }

        private void ShowNotice(string message)
        {
            // Dừng animation cũ nếu bấm mua liên tục
            _buyNotice.DOKill();

            _buyNotice.text = message;
            _buyNotice.gameObject.SetActive(true);

            // Reset alpha về 1
            Color color = _buyNotice.color;
            color.a = 1f;
            _buyNotice.color = color;

            // Đợi 0.7 giây -> fade trong 0.5 giây
            _buyNotice
                .DOFade(0f, 0.5f)
                .SetDelay(0.7f)
                .OnComplete(() =>
                {
                    _buyNotice.gameObject.SetActive(false);
                });
        }

        private void CoinShakeFX()
        {
            // Tránh nhiều hiệu ứng rung chồng lên nhau
            _coinUI.DOKill();

            // Rung trái - phải
            _coinUI.DOShakeAnchorPos(
                duration: 0.4f,
                strength: new Vector2(10f, 0f),
                vibrato: 15,
                randomness: 0f
            );
        }

        public override void Show(bool isShow)
        {
            base.Show(isShow);

            if (isShow)
            {
                coinTxt.text = Pref.Coin.ToString();
            }
        }

        public override void Close()
        {
            base.Close();
        }
    }
}