using UnityEngine;
using UnityEngine.UI;

namespace KHANH.FoodienGrillSort
{
    public class GUIManager : MonoBehaviour
    {
        private static GUIManager _instance;
        public static GUIManager Instance => _instance;

        public GameObject homeGUI;
        public GameObject gameGUI;
        public Dialog winDialog;
        public Dialog continueDialog;
        public Dialog loseDialog;
        public Dialog skipDeliveryDialog;
        public Text coinTxt;
        public Text heartTxt;
        public Text timeOfHeartTxt;
        private float timeSecond;
        private int timeOfOneHeart;

        private void Awake()
        {
            _instance = this;
            coinTxt.text = Pref.Coin.ToString();
            heartTxt.text = Pref.Heart.ToString();
        }

        private void Update()
        {
            if (Pref.Heart >= 5)
            {
                timeOfHeartTxt.text = "Đầy đủ";
                return;
            }

            timeSecond += Time.deltaTime;

            if (timeSecond >= 1f)
            {
                timeSecond -= 1f;
                timeOfOneHeart++;

                if (timeOfOneHeart >= 20)
                {
                    Pref.Heart++;
                    heartTxt.text = Pref.Heart.ToString();

                    timeOfOneHeart = 0;

                    if (Pref.Heart >= 5)
                    {
                        timeOfHeartTxt.text = "Đầy đủ";
                    }
                }
                else
                {
                    int remainTime = 20 - timeOfOneHeart;

                    int minute = remainTime / 60;
                    int second = remainTime % 60;

                    timeOfHeartTxt.text = minute + ":" + second.ToString("00");
                }
            }
        }

        public void ShowGameGUI(bool isShow)
        {
            if (gameGUI)
                gameGUI.SetActive(isShow);
            if (homeGUI)
            {
                homeGUI.SetActive(!isShow);
                coinTxt.text = Pref.Coin.ToString();
                heartTxt.text= Pref.Heart.ToString();
            }
                
        }

    }
}
