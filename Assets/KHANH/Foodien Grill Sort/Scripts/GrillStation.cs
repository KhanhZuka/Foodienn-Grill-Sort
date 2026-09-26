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
            // reset du lieu game cu
            _stackTrays.Clear();

            for (int i = 0; i < _totalSlot.Count; i++)
            {
                _totalSlot[i].OnReset();
            }

            for (int i = 0; i < _totalTrays.Count; i++)
            {
                _totalTrays[i].OnReset();
            }

            // xu ly set gia tri cho bep truoc, cai khay dau tien tren bep
            int foodCount = Random.Range(1, _totalSlot.Count + 1);
            List<Sprite> list = listFood;
            List<Sprite> listSlot = Utils.TakeAndRemoveRandom<Sprite>(list, foodCount);

            for (int i = 0; i < listSlot.Count; i++)
            {
                FoodSlot slot = this.RandomSlot();
                slot.OnSetSlot(listSlot[i]);
            }

            //xu ly dia
            List<List<Sprite>> remainFood = new List<List<Sprite>>();

            for (int i = 0; i < totalTray - 1; i++) // tru bo dia tren bep
            {
                if (listFood.Count <= 0)
                {
                    break;
                }
                remainFood.Add(new List<Sprite>());              
                int n = Random.Range(0, listFood.Count);
                remainFood[i].Add(listFood[n]);  //it nhat 1 dia co 1 do an
                listFood.RemoveAt(n);
            }

            //random thuc an con lai vao cac dia
            while (listFood.Count > 0)
            {
                // Chi lay nhung khay con cho
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
                    availableTrays[Random.Range(0, availableTrays.Count)];

                int n = Random.Range(0, listFood.Count);

                tray.Add(listFood[n]);
                listFood.RemoveAt(n);
            }

            for (int i = 0; i < _totalTrays.Count; i++)
            {
                bool active = i < remainFood.Count;
                _totalTrays[i].gameObject.SetActive(active);

                if (active)
                {
                    _totalTrays[i].OnSetFood(remainFood[i]);
                    Trayitem item = _totalTrays[i];
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
            if (this.GetSlotNull() == null) // kiem tra xem so luong slot du 3 item chua, neu chua du thi no == null
            {
                if (this.CanMerge())
                {
                    Debug.Log("Complete Grill");
                    StartCoroutine(IEMerge());

                    this.OnPrepareTray(false);
                    GameManager.Instance?.OnMinusFood();
                    AudioController.Instance.PlaySound(AudioController.Instance.MergeFood);
                }
            }

            IEnumerator IEMerge()
            {
                for (int i = 0; i < _totalSlot.Count; i++)
                {
                    _totalSlot[i].OnFadeOut();
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        public void OnCheckPrepareTray()
        {
            if (this.HasGrillEmpty())
            {
                this.OnPrepareTray(true);
            }
        }

        private void OnPrepareTray(bool isNow)
        {
            StartCoroutine(IEPrepare());

            IEnumerator IEPrepare()
            {
                if (!isNow)
                    yield return new WaitForSeconds(0.95f);

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
                    canvas.DOFade(0f, 0.5f).OnComplete(() =>
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
    }

}

