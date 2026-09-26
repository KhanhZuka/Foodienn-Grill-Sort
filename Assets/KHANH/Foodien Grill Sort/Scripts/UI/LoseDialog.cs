using UnityEngine;
using UnityEngine.UI;

namespace KHANH.FoodienGrillSort
{
    public class LoseDialog : Dialog
    {
        public Button CloseBtn;
        public Button ExitBtn;

        private void Start()
        {
            CloseBtn.onClick.AddListener(OnExit);
            ExitBtn.onClick.AddListener(OnExit);
        }

        public override void Show(bool isShow)
        {
            base.Show(isShow);
        }

        public override void Close()
        {
            base.Close();            
        }

        private void OnExit()
        {
            AudioController.Instance.PlaySound(AudioController.Instance.Bubble);
            Pref.Heart--;
            GUIManager.Instance.ShowGameGUI(false);
            gameObject.SetActive(false);
            AudioController.Instance.PlayMusic(AudioController.Instance.bgms, 0);
        }


    }
}
