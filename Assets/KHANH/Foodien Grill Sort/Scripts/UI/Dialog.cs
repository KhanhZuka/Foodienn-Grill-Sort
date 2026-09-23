using UnityEngine;

namespace KHANH.FoodienGrillSort
{
    public class Dialog : MonoBehaviour
    {
        public virtual void Show(bool isShow)
        {
            gameObject.SetActive(isShow);
        }

        public virtual void Close()
        {
            gameObject?.SetActive(false);
        }
    }
}


