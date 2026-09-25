using UnityEngine;
namespace KHANH.FoodienGrillSort
{
    public class SkipDeliveryDialog : Dialog
{
        public override void Show(bool isShow)
        {
            base.Show(isShow);
            Shipper.Instance.IsDelivering = false;
        }

        public override void Close()
        {
            base.Close();
            Shipper.Instance.IsDelivering = true;
        }
    }

}

