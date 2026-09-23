using UnityEngine;

namespace KHANH.FoodienGrillSort
{
    public class ContinueDialog : Dialog
    {
        public override void Show(bool isShow)
        {
            base.Show(isShow);
        }

        public void CloseDialog()
        {
            GUIManager.Instance.loseDialog.Show(true);
            gameObject.SetActive(false);
        }
    }
}
