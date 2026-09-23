using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KHANH.FoodienGrillSort
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        private int _allFood;
        private int _totalFood; // tong so loai thuc an, vi du 18/30
        private int _totalGrill; //tong so bep

        [SerializeField] private LevelData[] _levels;

        private LevelData _currentLevel;
        private int _currentLevelIndex;

        [SerializeField] private Transform _gridGrill;

        private List<GrillStation> _listGrills;
        private float _avgTray; // gia tri trung binh thuc an cho 1 dia
        private List<Sprite> _totalSpriteFood;

        [SerializeField] private Transform _magnetFX;
        [SerializeField] private List<Image> _magnetList;

        [SerializeField] private GridLayoutGroup _gridLayout;

        [SerializeField] private Text _foodProgressText;
        private int _remainFood;
        private int _completedFood = 0;

        [SerializeField] private Text _timeTxt;
        private int _levelTime;
        private int _minutes;
        private int _seconds;
        private float _timePerSecond = 0f;

        private bool _isPlaying;

        private void Awake()
        {
            _listGrills = Utils.GetListInChild<GrillStation>(_gridGrill);
            Sprite[] loadedSprite = Resources.LoadAll<Sprite>("Items");
            _totalSpriteFood = loadedSprite.ToList();//Linq
            _instance = this;

            LoadLevel(Pref.CurrentLevel);
            _minutes = _levelTime / 60;
            _seconds = _levelTime - _minutes * 60;

        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _foodProgressText.text = _completedFood.ToString() + "/" + _allFood.ToString();
            _timeTxt.text = _minutes.ToString() + ":" + _seconds.ToString();         
        }

        private void Update()
        {
            if (!_isPlaying) return;

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
                        _isPlaying = false;
                        GUIManager.Instance.continueDialog.Show(true);                       
                    }
                        
                }

            }
        }

        private void LoadLevel(int levelIndex)
        {
            _currentLevel = _levels[levelIndex];

             _allFood = _currentLevel.allFood;
             _totalFood = _currentLevel.totalFood;
             _totalGrill = _currentLevel.totalGrill;
             _levelTime = _currentLevel.levelTime;

            Debug.Log("Level: " + (levelIndex + 1));
        }

        private void UpdateTime(int minute, int second)
        {
            _timeTxt.text = minute.ToString() + ":" + second.ToString();
        }

        public void PlayGame()
        {
            _isPlaying = true;         
            ResetGame();
            LoadLevel(Pref.CurrentLevel);
            ResetGame();
            GUIManager.Instance.ShowGameGUI(true);           
            OnInitLevel();
        }

        private void ResetGame()
        {
            _remainFood = _allFood;
            _completedFood = 0;

            _minutes = _levelTime / 60;
            _seconds = _levelTime % 60;
            _timePerSecond = 0f;

            _foodProgressText.text = $"0/{_allFood}";
            UpdateTime(_minutes, _seconds);
        }

        private void OnInitLevel()
        {
            List<Sprite> takeFood = _totalSpriteFood.OrderBy(x => Random.value).Take(_totalFood).ToList(); // lay random so loai thuc an trong tong so loai thuc an
            List<Sprite> useFood = new List<Sprite>(); //takeFood*3

            for (int i = 0; i < _allFood; i++)
            {
                int n = i % takeFood.Count;
                for (int j = 0; j < 3; j++)
                {
                    useFood.Add(takeFood[n]);
                }
            }

            //random, trao vi tri cua cac item
            for (int i = 0; i < useFood.Count; i++)
            {
                int rand = Random.Range(i, useFood.Count);
                (useFood[i], useFood[rand]) = (useFood[rand], useFood[i]); // han nay la doi vi tri i hien tai cua vong lap va vi tri random, cach viet cua lamda
            }

            _avgTray = Random.Range(1.8f, 2.3f);
            int totalTray = Mathf.RoundToInt((float)useFood.Count / _avgTray); // tinh tong so dia

            List<int> trayPerGrill = this.DistributeEvelyn(_totalGrill, totalTray);
            List<int> foodPerGrill = this.DistributeEvelyn(_totalGrill, useFood.Count);

            for (int i = 0; i < _listGrills.Count; i++)
            {
                bool activeGrill = i < _totalGrill;
                _listGrills[i].gameObject.SetActive(activeGrill); //active nhung cai bep co trong Grid(noi chua cac cai bep)

                if (activeGrill)
                {
                    List<Sprite> lisFood = Utils.TakeAndRemoveRandom<Sprite>(useFood, foodPerGrill[i]); //lay ra cac hinh anh do an tren 1 bep
                    _listGrills[i].OnInitGrill(trayPerGrill[i], lisFood);
                }
            }
            UpdateGrillSize();
        }

        private List<int> DistributeEvelyn(int grillCount, int totalTrays) //chia deu cac khay vao cac bep
        {
            List<int> result = new List<int>();

            // tinh trung binh so luong dia
            float avg = (float)totalTrays / grillCount;  //3.5
            int low = Mathf.FloorToInt(avg);   //3
            int high = Mathf.CeilToInt(avg);   //4

            int hightCount = totalTrays - low * grillCount;
            int lowCount = grillCount - hightCount;

            for (int i = 0; i < lowCount; i++)
                result.Add(low);

            for (int i = 0; i < hightCount; i++)
                result.Add(high);

            //dao vi tri 
            for (int i = 0; i < result.Count; i++)
            {
                int rand = Random.Range(i, result.Count);
                (result[i], result[rand]) = (result[rand], result[i]);
            }

            return result;
        }


        public void OnMinusFood()
        {
            _remainFood--;
            _completedFood++;

            _foodProgressText.text =
                _completedFood + "/" + _allFood;

            if (_remainFood <= 0)
            {
                GUIManager.Instance.winDialog.Show(true);
                _isPlaying = false;
                Pref.CurrentLevel++;
                Debug.Log("Game complete");
            }
        }

        public void OnCheckAndShake()
        {
            Dictionary<string, List<FoodSlot>> groups = new Dictionary<string, List<FoodSlot>>();

            foreach (var grill in _listGrills)
            {
                if (grill.gameObject.activeInHierarchy)
                {
                    for (int i = 0; i < grill.TotalSlot.Count; i++)
                    {
                        FoodSlot slot = grill.TotalSlot[i];
                        if (slot.HasFood)
                        {
                            string name = slot.GetSpriteFood.name;
                            if (!groups.ContainsKey(name))
                                groups.Add(name, new List<FoodSlot>());

                            groups[name].Add(slot);
                        }
                    }
                }
            }

            foreach (var kvp in groups)
            {
                if (kvp.Value.Count >= 3)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        kvp.Value[i].DoShake();
                    }
                    return;
                }
            }
        }

        public void OnMagnet()
        {
            Dictionary<string, List<Image>> groups = new Dictionary<string, List<Image>>();

            foreach (var grill in _listGrills)
            {
                if (grill.gameObject.activeInHierarchy)
                {
                    for (int i = 0; i < grill.TotalSlot.Count; i++)
                    {
                        FoodSlot slot = grill.TotalSlot[i];
                        if (slot.HasFood)
                        {
                            string name = slot.GetSpriteFood.name;
                            if (!groups.ContainsKey(name))
                            {
                                groups.Add(name, new List<Image>());
                            }

                            groups[name].Add(slot.ImgFood);
                        }

                    }

                    Trayitem tray = grill.GetFirstTray();
                    if (tray != null)
                    {
                        for (int i = 0; i < tray.FoodList.Count; i++)
                        {
                            Image img = tray.FoodList[i];
                            if (img.gameObject.activeInHierarchy)
                            {
                                string name = img.sprite.name;
                                if (!groups.ContainsKey(name))
                                    groups.Add(name, new List<Image>());

                                groups[name].Add(img);
                            }
                        }
                    }
                }
            }

            StartCoroutine(IECollect());

            IEnumerator IECollect()
            {
                foreach (var kvp in groups)
                {
                    if (kvp.Value.Count >= 3)
                    {
                        _magnetFX.DOScale(Vector3.one, 0.4f);
                        yield return new WaitForSeconds(0.3f);

                        for (int i = 0; i < 3; i++)
                        {
                            Image imgDummy = _magnetList[i];
                            Image imgFood = kvp.Value[i];
                            imgDummy.sprite = imgFood.sprite;
                            imgDummy.SetNativeSize();
                            imgDummy.transform.position = imgFood.transform.position;
                            imgDummy.gameObject.SetActive(true);
                            imgFood.gameObject.SetActive(false);
                            imgDummy.color = new Color(1f, 1f, 1f, 1f);

                            Vector3 mid = (imgDummy.transform.position + _magnetFX.position) / 2f;
                            mid += new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
                            Vector3[] path = new Vector3[] { imgDummy.transform.position, mid, _magnetFX.position };

                            Sequence seq = DOTween.Sequence();
                            seq.Join(imgDummy.transform.DOPath(path, 1.5f, PathType.CatmullRom))
                                .Join(imgDummy.DOColor(new Color(1f, 1f, 1f, 0.1f), 1.5f))
                                .SetEase(Ease.OutQuad)
                                .OnComplete(() =>
                                {
                                    imgDummy.gameObject.SetActive(false);
                                    imgDummy.transform.localScale = Vector3.one;
                                    imgFood.gameObject.SendMessageUpwards("OnCheckPrepareTray");
                                });

                            yield return new WaitForSeconds(0.1f);
                        }

                        yield return new WaitForSeconds(1f);
                        _magnetFX.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
                        OnMinusFood();
                        yield break;
                    }
                }
            }
        }

        public void OnShuffle()
        {
            StartCoroutine(IEShuffle());

            IEnumerator IEShuffle()
            {
                List<Image> result = new List<Image>();

                foreach (var grill in _listGrills)
                {
                    if (grill.gameObject.activeInHierarchy)
                    {
                        result.AddRange(grill.ListFoodActive());
                        grill.OnShuffleFX();
                    }
                }

                yield return new WaitForSeconds(0.25f);
                for (int i = 0; i < result.Count; i++)
                {
                    int n = Random.Range(0, result.Count);
                    Sprite tmp = result[i].sprite;
                    result[i].sprite = result[n].sprite;
                    result[n].sprite = tmp;
                    result[i].SetNativeSize();
                    result[n].SetNativeSize();
                }
            }
        }

        public void OnAddMoreGrill()
        {
            foreach (var grill in _listGrills)
            {
                if (!grill.gameObject.activeInHierarchy)
                {
                    // Bật bếp mới
                    grill.gameObject.SetActive(true);

                    UpdateGrillSize();

                    // Chỉ thêm 1 bếp
                    break;
                }
            }
        }

        private void UpdateGrillSize()
        {
            int activeGrill = 0;

            foreach (var grill in _listGrills)
            {
                if (grill.gameObject.activeInHierarchy)
                    activeGrill++;
            }

            Vector2 cellSize;
            float grillScale;

            if (activeGrill <= 9)
            {
                cellSize = new Vector2(330f, 400f);
                grillScale = 1f;
            }
            else
            {
                cellSize = new Vector2(280f, 330f);
                grillScale = 0.82f;
            }

            _gridLayout.cellSize = cellSize;

            foreach (var grill in _listGrills)
            {
                if (grill.gameObject.activeInHierarchy)
                {
                    grill.transform
                        .DOScale(Vector3.one * grillScale, 0.3f)
                        .SetEase(Ease.OutQuad);
                }
            }
        }

    }


}

