using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace KHANH.FoodienGrillSort
{
    public class SkipDeliveryDialog : Dialog
{
        [SerializeField] private Image _insufficientCoinsNotice;

        public override void Show(bool isShow)
        {
            base.Show(isShow);

            Shipper.Instance.IsDelivering = false;
            GameManager.Instance.IsPlaying = false;

            AudioController.Instance.PauseMusic();
        }

        public override void Close()
        {
            AudioController.Instance.PlaySound(
                AudioController.Instance.Bubble
            );

            base.Close();

            GameManager.Instance.IsPlaying = true;
            Shipper.Instance.IsDelivering = true;

            AudioController.Instance.ResumeMusic();
        }

        public void CloseContinueDialog()
        {
            if (Pref.Coin >= 30)
            {
                AudioController.Instance.PlaySound(AudioController.Instance.Bubble);
                Shipper.Instance.HideCustomer();
                Shipper.Instance.RemoveShipperOrder();
                gameObject?.SetActive(false);
                GameManager.Instance.IsPlaying = true;
                Pref.Coin -= 30;
                GUIManager.Instance.UpdateHomeCoin();              
                AudioController.Instance.ResumeMusic();
            }
            else
            {
                // Debug.Log("Ban khong du coin");
                _insufficientCoinsNotice.gameObject.SetActive(true);
                StartCoroutine(IENotice());

                IEnumerator IENotice()
                {
                    yield return new WaitForSeconds(1);
                    _insufficientCoinsNotice.gameObject.SetActive(false);
                }
            }
        }
    }

}

