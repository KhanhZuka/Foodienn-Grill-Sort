using UnityEngine;

namespace KHANH.FoodienGrillSort
{
    public class ContinueDialog : Dialog
    {
        public override void Show(bool isShow)
        {
            base.Show(isShow);
        }

        public override void Close()
        {
            base.Close();
            AudioController.Instance.PlaySound(AudioController.Instance.Bubble);
            GUIManager.Instance.loseDialog.Show(true);           
        }

        public void CloseContinueDialog()
        {
            if(Pref.Coin >= 30)
            {
                AudioController.Instance.PlaySound(AudioController.Instance.Bubble);
                gameObject?.SetActive(false);
                GameManager.Instance._seconds = 45;
                GameManager.Instance._isPlaying = true;
                Pref.Coin -= 30;
                GUIManager.Instance.homePanel.UpdateHomeCoin();
                AudioController.Instance.PlayMusic(AudioController.Instance.bgms,1);
            }
            else
            {
                Debug.Log("Ban khong du coin");
            }
        }
    }
}
