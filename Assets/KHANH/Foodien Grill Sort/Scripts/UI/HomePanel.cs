using UnityEngine;
using UnityEngine.UI;

namespace KHANH.FoodienGrillSort
{
    public class HomePanel : MonoBehaviour
    {
        public RectTransform Selected;
        public Button ShopBtn;
        public Button HomeBtn;
        public Button RankBtn;
        [SerializeField] private Text coinTxt;

        private void Awake()
        {
            SetSelectedItem(1);
            coinTxt.text = Pref.Coin.ToString();
        }

        private void Start()
        {

            ShopBtn.onClick.AddListener(() => SetSelectedItem(0));
            HomeBtn.onClick.AddListener(() => SetSelectedItem(1));
            RankBtn.onClick.AddListener(() => SetSelectedItem(2));
        }

        private void SetSelectedItem(int index)
        {
            ColorUtility.TryParseHtmlString("#00B3FF", out Color selectedColor);
            ColorUtility.TryParseHtmlString("#3B6FC5", out Color normalColor);

            for (int i = 0; i < Selected.childCount; i++)
            {
                Transform item = Selected.GetChild(i);

                Image background = item.GetComponent<Image>();
                LayoutElement layout = item.GetComponent<LayoutElement>();
                Text text = item.GetComponentInChildren<Text>(true);
                Image childImg = item.GetChild(1).GetComponent<Image>();

                if (layout == null)
                {
                    Debug.LogError("Thiếu LayoutElement: " + item.name);
                    continue;
                }

                if (i == index)
                {
                    // Mau item dang chon
                    background.color = selectedColor;

                    // Gian chieu rong
                    layout.preferredWidth =
                        Selected.rect.width - 2 * 343f;

                    if (text != null)
                        text.gameObject.SetActive(true);

                    // Icon to hon + di len
                    childImg.rectTransform.localScale =
                        Vector3.one * 1.3f;

                    childImg.rectTransform.anchoredPosition =
                        new Vector2(0, 90f);
                }
                else
                {
                    // Màu item binh thuong
                    background.color = normalColor;

                    layout.preferredWidth = 343f;

                    if (text != null)
                        text.gameObject.SetActive(false);

                    childImg.rectTransform.localScale = Vector3.one;
                    childImg.rectTransform.anchoredPosition = Vector2.zero;
                }
            }
        }

        public void UpdateHomeCoin()
        {
            coinTxt.text = Pref.Coin.ToString();
        }
    }
}
