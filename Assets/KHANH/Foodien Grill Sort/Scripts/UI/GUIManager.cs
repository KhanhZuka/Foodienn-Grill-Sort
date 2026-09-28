using UnityEngine;
using UnityEngine.UI;

namespace KHANH.FoodienGrillSort
{
    public class GUIManager : MonoBehaviour
    {
        private static GUIManager _instance;
        public static GUIManager Instance => _instance;

        public GameObject HomeGUI;
        public GameObject GameGUI;

        public Dialog WinDialog;
        public ContinueDialog ContinueDialog;
        public Dialog LoseDialog;
        public Dialog SkipDeliveryDialog;

        public HomePanel HomePanel;
        [SerializeField] private Text _coinTxt;
        public Text CoinTxt => _coinTxt;
        [SerializeField] private Text _levelTxt;
        public Text LevelTxt => _levelTxt;

        public Text HeartTxt;
        public Text TimeOfHeartTxt;
        private float _timeSecond;
        private int _timeOfOneHeart;

        public Image CompleteGame;

        private void Awake()
        {
            _instance = this;
            _coinTxt.text = Pref.Coin.ToString();
            HeartTxt.text = Pref.Heart.ToString();
            _levelTxt.text = "Cấp độ " + (Pref.CurrentLevel + 1).ToString();
        }

        private void Update()
        {
            if (Pref.Heart >= 5)
            {
                TimeOfHeartTxt.text = "Đầy đủ";
                return;
            }

            _timeSecond += Time.deltaTime;

            if (_timeSecond >= 1f)
            {
                _timeSecond -= 1f;
                _timeOfOneHeart++;

                if (_timeOfOneHeart >= 20)
                {
                    Pref.Heart++;
                    HeartTxt.text = Pref.Heart.ToString();

                    _timeOfOneHeart = 0;

                    if (Pref.Heart >= 5)
                    {
                        TimeOfHeartTxt.text = "Đầy đủ";
                    }
                }
                else
                {
                    int remainTime = 20 - _timeOfOneHeart;

                    int minute = remainTime / 60;
                    int second = remainTime % 60;

                    TimeOfHeartTxt.text = minute + ":" + second.ToString("00");
                }
            }
        }

        public void ShowGameGUI(bool isShow)
        {
            if (GameGUI)
                GameGUI.SetActive(isShow);
            if (HomeGUI)
            {
                HomeGUI.SetActive(!isShow);
                CoinTxt.text = Pref.Coin.ToString();
                HeartTxt.text= Pref.Heart.ToString();
            }
                
        }

        public void UpdateHomeCoin()
        {
            CoinTxt.text = Pref.Coin.ToString();
        }

        public void UpdateHomeHeart()
        {
            HeartTxt.text = Pref.Heart.ToString();
        }

    }
}
