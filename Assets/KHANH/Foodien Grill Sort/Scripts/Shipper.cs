using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;


namespace KHANH.FoodienGrillSort
{
    public class Shipper : MonoBehaviour
    {
        private static Shipper _instance;
        public static Shipper Instance => _instance;

        [SerializeField] private RectTransform _customer;
        [SerializeField] private RectTransform _orderNotice;
        [SerializeField] private List<Image> _orderList;
        private List<Sprite> _requiredFoods = new List<Sprite>();
        [SerializeField] private Text _shipperTimeTxt;

        [SerializeField] private float _startX = -100f;
        [SerializeField] private float _targetX = 0f;
        [SerializeField] private float _moveTime = 0.6f;
        private bool _isDelivering;
        public bool IsDelivering
        {
            get => _isDelivering;
            set => _isDelivering = value;
        }
        private int _waitTimeOfShipper;
        private int _minutes;
        private int _seconds;

        public int Second
        {
            get => _seconds;
            set => _seconds = value;
        }

        private float _timePerSecond = 0f;

        private void Awake()
        {
            _instance = this;           
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (!_isDelivering) return;

            _timePerSecond += Time.deltaTime;
            if (_timePerSecond >= 1f)
            {
                _timePerSecond = 0.0f;
                if (_seconds > 0)
                {
                    _seconds--;
                    UpdateTime(_minutes, _seconds);
                }
                else
                {
                    if (_minutes > 0)
                    {
                        _seconds = 59;
                        _minutes--;
                        UpdateTime(_minutes, _seconds);
                    }
                    else
                    {
                        AudioController.Instance.PauseMusic();
                        _isDelivering = false;
                        //Shipper.Instance.HideCustomer();
                        GUIManager.Instance.ContinueDialog.ShowShipperDialog();                       
                        AudioController.Instance.PlaySound(AudioController.Instance.LoseGame);                      
                    }

                }

            }
        }

        private void UpdateTime(int minute, int second)
        {
            _shipperTimeTxt.text = minute.ToString() + ":" + second.ToString();
        }

        public void ShowCustomer()
        {
            _isDelivering = true;
            _waitTimeOfShipper = GameManager.Instance.WaitShipperTime;
            _minutes = _waitTimeOfShipper / 60;
            _seconds = _waitTimeOfShipper - _minutes * 60;
            _shipperTimeTxt.text = _minutes.ToString() + ":" + _seconds.ToString();

            Vector2 pos = _customer.anchoredPosition;
            pos.x = _startX;
            _customer.anchoredPosition = pos;

            _orderNotice.localScale = Vector3.zero;
           
            List<Sprite> orders = GameManager.Instance.OnShipper();
            _requiredFoods.Clear();
            _requiredFoods.AddRange(orders);

            for (int i = 0; i < _orderList.Count; i++)
            {
                _orderList[i].gameObject.SetActive(false);
                _orderList[i].transform.GetChild(1).GetComponent<Image>().gameObject.SetActive(false);
            }

            for (int i = 0; i < orders.Count; i++)
            {
                Image foodImg = _orderList[i].transform.GetChild(0).GetComponent<Image>();

                foodImg.sprite = orders[i];

                _orderList[i].gameObject.SetActive(true);
            }

            AudioController.Instance.PlaySound(AudioController.Instance.Bip);

            _customer.DOAnchorPosX(_targetX, _moveTime)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    _orderNotice.DOScale(Vector3.one, 0.1f)
                        .SetEase(Ease.OutBack);
                });
        }

        public void HideCustomer()
        {
            _isDelivering = false;
            _orderNotice.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    _customer.DOAnchorPosX(_startX, _moveTime)
                        .SetEase(Ease.InBack);
                });
        }

        public void OnFoodCompleted(Sprite food)
        {
            if (food == null)
                return;

            if (!_requiredFoods.Contains(food))
                return;

            _requiredFoods.Remove(food);

            for (int i = 0; i < _orderList.Count; i++)
            {
                Image orderItem = _orderList[i];

                if (orderItem == null)
                    continue;

                Image foodImage =
                    orderItem.transform.GetChild(0).GetComponent<Image>();

                if (foodImage == null || foodImage.sprite != food)
                    continue;

                Image completedImage =
                    orderItem.transform.GetChild(1).GetComponent<Image>();

                if (completedImage != null)
                {
                    completedImage.gameObject.SetActive(true);
                }

                break;
            }

            if (_requiredFoods.Count == 0)
            {
                AudioController.Instance.PlaySound(
                    AudioController.Instance.ThankYou
                );

                HideCustomer();
            }
        }

        public void RemoveShipperOrder()
        {
            _requiredFoods.Clear();
        }
    }
}


