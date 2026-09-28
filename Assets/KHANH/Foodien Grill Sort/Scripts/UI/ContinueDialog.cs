using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace KHANH.FoodienGrillSort
{
    public enum ContinueType
    {
        Game,
        Shipper
    }

    public class ContinueDialog : Dialog
    {
        [SerializeField] private Image _insufficientCoinsNotice;

        private ContinueType _continueType;


        public void ShowGameDialog()
        {
            _continueType = ContinueType.Game;

            Show(true);

            GameManager.Instance.IsPlaying = false;
        }


        public void ShowShipperDialog()
        {
            _continueType = ContinueType.Shipper;

            Show(true);

            GameManager.Instance.IsPlaying = false;
        }


        public void CloseContinueDialog()
        {
            if (Pref.Coin < 30)
            {
                ShowInsufficientCoinsNotice();
                return;
            }

            AudioController.Instance.PlaySound(
                AudioController.Instance.Bubble
            );

            Pref.Coin -= 30;

            GUIManager.Instance.UpdateHomeCoin();

            ContinueGame();

            gameObject.SetActive(false);

            AudioController.Instance.ResumeMusic();
        }


        private void ContinueGame()
        {
            switch (_continueType)
            {
                case ContinueType.Game:
                    GameManager.Instance.Second = 45;
                    break;

                case ContinueType.Shipper:
                    Shipper.Instance.Second = 45;
                    Shipper.Instance.IsDelivering = true;
                    break;
            }

            GameManager.Instance.IsPlaying = true;
        }


        private void ShowInsufficientCoinsNotice()
        {
            StartCoroutine(IENotice());
        }


        private IEnumerator IENotice()
        {
            _insufficientCoinsNotice.gameObject.SetActive(true);

            yield return new WaitForSeconds(1f);

            _insufficientCoinsNotice.gameObject.SetActive(false);
        }


        public override void Close()
        {
            base.Close();

            AudioController.Instance.PlaySound(
                AudioController.Instance.Bubble
            );

            GUIManager.Instance.LoseDialog.Show(true);
        }
    }
}