using UnityEngine;

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

        private void Awake()
        {
            _instance = this;
        }

        public void ShowGameGUI(bool isShow)
        {
            if (gameGUI)
                gameGUI.SetActive(isShow);
            if (homeGUI)
                homeGUI.SetActive(!isShow);
        }

    }
}
