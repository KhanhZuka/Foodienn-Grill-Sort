using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KHANH.FoodienGrillSort
{
    public class GrillStation : MonoBehaviour
    {
        [SerializeField] private Transform _trayContainer;
        [SerializeField] private Transform _slotContainer;

        private List<Trayitem> _totalTrays;
        private List<FoodSlot> _totalSlot;

        private Stack<Trayitem> _stackTrays = new Stack<Trayitem>();

        public List<FoodSlot> TotalSlot => _totalSlot;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            _totalTrays = Utils.GetListInChild<Trayitem>(_trayContainer);
            _totalSlot = Utils.GetListInChild<FoodSlot>(_slotContainer);
        }

        public void OnInitGrill(int totalTray, List<Sprite> listFood)
        {
            // Reset dữ liệu game cũ
            _stackTrays.Clear();

            for (int i = 0; i < _totalSlot.Count; i++)
            {
                _totalSlot[i].OnReset();
            }

            for (int i = 0; i < _totalTrays.Count; i++)
            {
                _totalTrays[i].OnReset();
            }


            // =========================================
            // XỬ LÝ FOOD TRÊN BẾP
            // =========================================

            // Vì totalTray của bạn có tính cả "khay trên bếp"
            // nên số khay thực sự bên dưới là totalTray - 1
            int trayCount = totalTray - 1;

            // Một khay chứa tối đa 3 food
            int trayCapacity = trayCount * 3;

            // Số food tối thiểu phải nằm trên bếp
            int minFoodOnGrill = Mathf.Max(
                1,
                listFood.Count - trayCapacity
            );

            // Random số food trên bếp
            int foodCount = Random.Range(
                minFoodOnGrill,
                _totalSlot.Count + 1
            );


            // DEBUG
            Debug.Log(
                $"{gameObject.name} | " +
                $"Food: {listFood.Count} | " +
                $"Tray dưới: {trayCount} | " +
                $"TrayCapacity: {trayCapacity} | " +
                $"MinFoodOnGrill: {minFoodOnGrill} | " +
                $"FoodOnGrill: {foodCount}"
            );


            List<Sprite> list = listFood;

            List<Sprite> listSlot =
                Utils.TakeAndRemoveRandom<Sprite>(
                    list,
                    foodCount
                );


            // Đặt food lên các slot của bếp
            for (int i = 0; i < listSlot.Count; i++)
            {
                FoodSlot slot = RandomSlot();

                slot.OnSetSlot(listSlot[i]);
            }


            // =========================================
            // XỬ LÝ FOOD TRONG CÁC KHAY
            // =========================================

            List<List<Sprite>> remainFood =
                new List<List<Sprite>>();

            for (int i = 0; i < totalTray - 1; i++)
            {
                if (listFood.Count <= 0)
                {
                    break;
                }

                remainFood.Add(new List<Sprite>());

                int n = Random.Range(
                    0,
                    listFood.Count
                );

                // Mỗi khay ít nhất có 1 food
                remainFood[i].Add(listFood[n]);

                listFood.RemoveAt(n);
            }


            // Random các food còn lại vào khay
            while (listFood.Count > 0)
            {
                List<List<Sprite>> availableTrays =
                    remainFood.FindAll(x => x.Count < 3);

                if (availableTrays.Count == 0)
                {
                    Debug.LogError(
                        $"Không đủ khay! Còn dư {listFood.Count} food"
                    );

                    break;
                }

                List<Sprite> tray =
                    availableTrays[
                        Random.Range(0, availableTrays.Count)
                    ];

                int n = Random.Range(
                    0,
                    listFood.Count
                );

                tray.Add(listFood[n]);

                listFood.RemoveAt(n);
            }


            // =========================================
            // SET FOOD CHO TRAYITEM
            // =========================================

            for (int i = 0; i < _totalTrays.Count; i++)
            {
                bool active =
                    i < remainFood.Count;

                _totalTrays[i]
                    .gameObject
                    .SetActive(active);

                if (active)
                {
                    _totalTrays[i]
                        .OnSetFood(remainFood[i]);

                    Trayitem item =
                        _totalTrays[i];

                    _stackTrays.Push(item);
                }
            }
        }

        private FoodSlot RandomSlot()
        {
        reRand: int n = Random.Range(0, _totalSlot.Count);
            if (_totalSlot[n].HasFood) goto reRand;

            return _totalSlot[n];
        }



        public FoodSlot GetSlotNull() //Neu tat ca cac o deu co do an tra ve == null
        {
            FoodSlot tmp = null;
            for (int i = 0; i < _totalSlot.Count; i++)
            {
                if (!_totalSlot[i].HasFood)
                {
                    return _totalSlot[i];
                }
            }
            return tmp;
        }

        private bool HasGrillEmpty()
        {
            for (int i = 0; i < _totalSlot.Count; i++)
            {
                if (_totalSlot[i].HasFood)
                    return false;
            }
            return true;
        }

        public void OnCheckMerge()
        {
            if (GetSlotNull() == null)
            {
                if (CanMerge())
                {
                    Debug.Log("Complete Grill");

                    StartCoroutine(IEMerge());

                    GameManager.Instance?.OnMinusFood();
                    AudioController.Instance.PlaySound(
                        AudioController.Instance.MergeFood
                    );
                }
            }

            IEnumerator IEMerge()
            {
                // Cho cả 3 món biến mất cùng lúc
                for (int i = 0; i < _totalSlot.Count; i++)
                {
                    _totalSlot[i].OnFadeOut();
                }

                // OnFadeOut mất 0.6s
                yield return new WaitForSeconds(0.6f);

                // Lúc này món cũ đã biến mất hoàn toàn
                OnPrepareTray();
            }
        }

        public void OnCheckPrepareTray()
        {
            if (HasGrillEmpty())
            {
                OnPrepareTray();
            }
        }

        private void OnPrepareTray()
        {
            StartCoroutine(IEPrepare());

            IEnumerator IEPrepare()
            {
                if (_stackTrays.Count > 0)
                {
                    Trayitem item = _stackTrays.Pop();

                    for (int i = 0; i < item.FoodList.Count; i++)
                    {
                        Image img = item.FoodList[i];

                        if (img.gameObject.activeInHierarchy)
                        {
                            _totalSlot[i].OnPrepareItem(img);
                            img.gameObject.SetActive(false);

                            yield return new WaitForSeconds(0.1f);
                        }
                    }

                    CanvasGroup canvas = item.GetComponent<CanvasGroup>();

                    canvas.DOFade(0f, 0.5f)
                        .OnComplete(() =>
                        {
                            item.gameObject.SetActive(false);
                            canvas.alpha = 1f;
                        });
                }
            }
        }

        private bool CanMerge()
        {
            string name = _totalSlot[0].GetSpriteFood.name;
            for (int i = 1; i < _totalSlot.Count; i++)
            {
                if (_totalSlot[i].GetSpriteFood.name != name)
                    return false;
            }
            Shipper.Instance.OnFoodCompleted(_totalSlot[0].GetSpriteFood);
            return true;
        }

        public Trayitem GetFirstTray()
        {
            if (_stackTrays.Count > 0)
                return _stackTrays.Peek();

            return null;
        }

        public List<Image> ListFoodActive()
        {
            List<Image> result = new List<Image>();

            for (int i = 0; i < _totalSlot.Count; i++)
            {
                if (_totalSlot[i].HasFood)
                    result.Add(_totalSlot[i].ImgFood);
            }

            for (int i = 0; i < _totalTrays.Count; i++)
            {
                Trayitem tray = _totalTrays[i];
                if (tray.gameObject.activeInHierarchy)
                {
                    for (int j = 0; j < tray.FoodList.Count; j++)
                    {
                        if (tray.FoodList[j].gameObject.activeInHierarchy)
                            result.Add(tray.FoodList[j]);
                    }
                }
            }

            return result;
        }

        public void OnShuffleFX()
        {
            // Food trên bếp
            for (int i = 0; i < _totalSlot.Count; i++)
            {
                if (_totalSlot[i].HasFood)
                {
                    ShuffleFoodFX(
                        _totalSlot[i].ImgFood.transform,
                        1f
                    );
                }
            }

            // Food ở các khay dưới
            for (int i = 0; i < _totalTrays.Count; i++)
            {
                Trayitem tray = _totalTrays[i];

                if (tray.gameObject.activeInHierarchy)
                {
                    for (int j = 0; j < tray.FoodList.Count; j++)
                    {
                        Image img = tray.FoodList[j];

                        if (img.gameObject.activeInHierarchy)
                        {
                            ShuffleFoodFX(
                                img.transform,
                                0.5f
                            );
                        }
                    }
                }
            }
        }

        private void ShuffleFoodFX(Transform food, float targetScale)
        {
            food.DOKill();

            food.DOScale(Vector3.zero, 0.25f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    food.DOScale(Vector3.one * targetScale, 0.25f)
                        .SetEase(Ease.OutBack);
                });
        }

        public void OnCheckFirstTray()
        {
            if (_stackTrays.Count <= 0)
                return;

            Trayitem firstTray = _stackTrays.Peek();

            // Khay đầu vẫn còn đồ ăn -> không làm gì
            if (firstTray.HasFood)
                return;

            // Khay đầu đã hết đồ ăn
            _stackTrays.Pop();

            firstTray.gameObject.SetActive(false);

            // Sau Pop(), Peek() bây giờ chính là khay tiếp theo
            if (_stackTrays.Count > 0)
            {
                Trayitem nextTray = _stackTrays.Peek();

                nextTray.gameObject.SetActive(true);
            }
        }
    }

}

