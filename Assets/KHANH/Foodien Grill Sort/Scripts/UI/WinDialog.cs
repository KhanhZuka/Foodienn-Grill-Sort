using UnityEngine;

namespace KHANH.FoodienGrillSort
{
    public class WinDialog : Dialog
    {
        public override void Show(bool isShow)
        {
            base.Show(isShow);
        }

        public override void Close()
        {
            AudioController.Instance.PlaySound(AudioController.Instance.Bubble);
            base.Close();           
            AudioController.Instance.PlayMusic(AudioController.Instance.bgms, 0);
        }
    }
}


