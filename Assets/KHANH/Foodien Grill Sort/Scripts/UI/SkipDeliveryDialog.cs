using UnityEngine;
namespace KHANH.FoodienGrillSort
{
    public class SkipDeliveryDialog : Dialog
{
        public override void Show(bool isShow)
        {
            base.Show(isShow);
            Shipper.Instance.IsDelivering = false;
            AudioController.Instance.StopMusic();
            GameManager.Instance._isPlaying = false;
        }

        public override void Close()
        {
            AudioController.Instance.PlaySound(AudioController.Instance.Bubble);
            base.Close();
            GameManager.Instance._isPlaying = true;
            Shipper.Instance.IsDelivering = true;
            AudioController.Instance.PlayMusic(AudioController.Instance.bgms, 1);
        }

        public void CloseContinueDialog()
        {
            if (Pref.Coin >= 30)
            {
                AudioController.Instance.PlaySound(AudioController.Instance.Bubble);
                gameObject?.SetActive(false);
                GameManager.Instance._isPlaying = true;
                Pref.Coin -= 30;
                GUIManager.Instance.homePanel.UpdateHomeCoin();
                Shipper.Instance.HideCustomer();
                //AudioController.Instance.PlayMusic(AudioController.Instance.bgms, 1);
            }
            else
            {
                Debug.Log("Ban khong du coin");
            }
        }
    }

}

