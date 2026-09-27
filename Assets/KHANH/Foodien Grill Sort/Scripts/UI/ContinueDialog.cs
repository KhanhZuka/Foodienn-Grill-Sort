using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace KHANH.FoodienGrillSort
{
    public class ContinueDialog : Dialog
    {
        [SerializeField] private Image insufficientCoinsNotice;
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
                GUIManager.Instance.UpdateHomeCoin();
                AudioController.Instance.PlayMusic(AudioController.Instance.bgms,1);
            }
            else
            {
                //Debug.Log("Ban khong du coin");
                insufficientCoinsNotice.gameObject.SetActive(true);
                StartCoroutine(IENotice());

                IEnumerator IENotice(){
                    yield return new WaitForSeconds(1);
                    insufficientCoinsNotice.gameObject.SetActive(false);
                }
            }
        }
    }
}
