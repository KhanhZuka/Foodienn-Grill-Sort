using UnityEngine;
namespace KHANH.FoodienGrillSort
{
    public class PuaseDialog : Dialog
    {
        public override void Show(bool isShow)
        {
            base.Show(isShow);
            AudioController.Instance.PlaySound(AudioController.Instance.Bubble);
            GameManager.Instance._isPlaying = false;
        }

        public override void Close()
        {
            base.Close();
            AudioController.Instance.PlaySound(AudioController.Instance.Bubble);
            GameManager.Instance._isPlaying = true;
        }

        public void ClosePauseAndGameGui()
        {
            AudioController.Instance.PlaySound(AudioController.Instance.Bubble);
            gameObject?.SetActive(false);
            GUIManager.Instance.ShowGameGUI(false);
            AudioController.Instance.PlayMusic(AudioController.Instance.bgms, 0);
        }
    }
}


