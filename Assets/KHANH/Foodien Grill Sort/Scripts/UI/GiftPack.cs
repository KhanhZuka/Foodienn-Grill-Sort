using UnityEngine;
using UnityEngine.UI;
namespace KHANH.FoodienGrillSort
{
    public class GiftPack : MonoBehaviour
    {
        [SerializeField] private Button _buyBtn;
        public Button BuyBtn => _buyBtn;
        [SerializeField] private int _price;
        public int Price => _price;
        [SerializeField] private int _magnetAmount;
        public int MagnetAmount => _magnetAmount;
        [SerializeField] private int _shuffleAmount;
        public int ShuffleAmount => _shuffleAmount;
        [SerializeField] private int _extraGrillAmount;
        public int ExtraGrillAmount => _extraGrillAmount;
        [SerializeField] private int _heartAmount;
        public int HeartAmount => _heartAmount;
    }
}


