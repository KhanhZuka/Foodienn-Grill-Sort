using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private int _levelTime;
        private int _coin;
        private int _shipperTriggerFoodCount;
        private int _requiredFoodCount;
        private int _waitShipperTime;
        public int WaitShipperTime => _waitShipperTime;

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
        private int _minutes;
        public int Second;
        private float _timePerSecond = 0f;

        [SerializeField] private Text _magnetTxt;
        [SerializeField] private Text _shuffleTxt;
        [SerializeField] private Text _extraGrillTxt;

        private bool _isPlaying;
        public bool IsPlaying;

        [SerializeField] private Text _levelText;

        private void Awake()
        {
            _listGrills = Utils.GetListInChild<GrillStation>(_gridGrill);
            Sprite[] loadedSprite = Resources.LoadAll<Sprite>("Items");
            _totalSpriteFood = loadedSprite.ToList();//Linq
            _instance = this;

            LoadLevel(Pref.CurrentLevel);
            _minutes = _levelTime / 60;
            Second = _levelTime - _minutes * 60;
            
            UpdateBoosterUI();
        }

        void Start()
        {
            _foodProgressText.text = _completedFood.ToString() + "/" + _allFood.ToString();
            _timeTxt.text = _minutes.ToString() + ":" + Second.ToString();
        }

        private void Update()
        {
            if (!IsPlaying) return;

            _timePerSecond += Time.deltaTime;
            if (_timePerSecond >= 1f)
            {
                _timePerSecond = 0.0f;
                if (Second > 0)
                {
                    Second--;
                    UpdateTime();
                }
                else
                {
                    if (_minutes > 0)
                    {
                        Second = 59;
                        _minutes--;
                        UpdateTime();
                    }
                    else
                    {
                        IsPlaying = false;
                        AudioController.Instance.PauseMusic();                       
                        GUIManager.Instance.ContinueDialog.ShowGameDialog();
                        AudioController.Instance.PlaySound(AudioController.Instance.LoseGame);
                    }

                }

            }
        }

        private void LoadLevel(int levelIndex)
        {
            _currentLevel = _levels[levelIndex];

            _allFood = _currentLevel.AllFood;
            _totalFood = _currentLevel.TotalFood;
            _totalGrill = _currentLevel.TotalGrill;
            _levelTime = _currentLevel.LevelTime;
            _coin = _currentLevel.Coin;
            _shipperTriggerFoodCount = _currentLevel.ShipperTriggerFoodCount;
            _requiredFoodCount = _currentLevel.RequiredFoodCount;
            _waitShipperTime = _currentLevel.ShipperTime;
            
            UpdateBoosterUI();

            Debug.Log("Level: " + (levelIndex + 1));
        }

        private void UpdateTime()
        {
            _timeTxt.text = $"{_minutes:00}:{Second:00}";
        }

        public void PlayGame()
        {
            if(Pref.CurrentLevel <= 9)
            {
                if (AudioController.Instance != null)
                {
                    AudioController.Instance.PlaySound(AudioController.Instance.Bubble);
                    AudioController.Instance.PlayMusic(AudioController.Instance.Bgms, 1);
                }
                IsPlaying = true;
               // ResetGame();
                LoadLevel(Pref.CurrentLevel);
                ResetGame();
                GUIManager.Instance.ShowGameGUI(true);
                Shipper.Instance.HideCustomer();
                OnInitLevel();
                _levelText.text += (Pref.CurrentLevel + 1).ToString();
            }
        }

        private void ResetGame()
        {
            _remainFood = _allFood;
            _completedFood = 0;

            _minutes = _levelTime / 60;
            Second = _levelTime % 60;
            _timePerSecond = 0f;

            _foodProgressText.text = $"0/{_allFood}";
            UpdateTime();
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

            _avgTray = Random.Range(1.7f, 2.0f);
            int totalTray = Mathf.RoundToInt((float)useFood.Count / _avgTray); // tinh tong so dia

            List<int> trayPerGrill = this.DistributeItemsEvenly(_totalGrill, totalTray);
            List<int> foodPerGrill = this.DistributeItemsEvenly(_totalGrill, useFood.Count);

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

        private List<int> DistributeItemsEvenly(int grillCount, int totalTrays) //chia deu cac khay vao cac bep
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

            if (_completedFood == _shipperTriggerFoodCount)
            {
                StartCoroutine(IEShowCustomer());

                IEnumerator IEShowCustomer()
                {
                    yield return new WaitForSeconds(1);
                    Shipper.Instance.ShowCustomer();
                }
            }
                

            _foodProgressText.text =
                _completedFood + "/" + _allFood;          

            if (_remainFood <= 0)
            {
                AudioController.Instance.PauseMusic();
                GUIManager.Instance.WinDialog.Show(true);
                IsPlaying = false;
                Pref.CurrentLevel++;
                Pref.Coin += _coin;
                AudioController.Instance.PlaySound(AudioController.Instance.LevelComplete);   
                GUIManager.Instance.LevelTxt.text = "Cấp độ " + (Pref.CurrentLevel + 1).ToString();
                if(Pref.CurrentLevel >= 9)
                {
                    GUIManager.Instance.CompleteGame.gameObject.SetActive(true);
                }               
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
            if (Pref.Magnet <= 0)
                return;

            Dictionary<string, List<Image>> foodGroups = GetAvailableFoodGroups();

            List<Image> targetFoods = FindMagnetTarget(foodGroups);

            if (targetFoods == null)
            {
                Debug.Log("Không có 3 món giống nhau để hút");
                return;
            }

            UseMagnetBooster();

            StartCoroutine(IECollectWithMagnet(targetFoods));
        }


        // Tìm tất cả food mà Magnet có thể hút
        private Dictionary<string, List<Image>> GetAvailableFoodGroups()
        {
            Dictionary<string, List<Image>> foodGroups = new Dictionary<string, List<Image>>();

            foreach (GrillStation grill in _listGrills)
            {
                if (!grill.gameObject.activeInHierarchy)
                    continue;

                AddFoodFromSlots(grill, foodGroups);
                AddFoodFromFirstTray(grill, foodGroups);
            }

            return foodGroups;
        }


        // Lấy food đang nằm trên bếp
        private void AddFoodFromSlots(GrillStation grill,Dictionary<string, List<Image>> foodGroups)
        {
            foreach (FoodSlot slot in grill.TotalSlot)
            {
                if (!slot.HasFood)
                    continue;

                AddFoodToGroup(slot.ImgFood, foodGroups);
            }
        }


        // Lấy food ở khay đầu tiên
        private void AddFoodFromFirstTray(GrillStation grill,Dictionary<string, List<Image>> foodGroups)
        {
            Trayitem tray = grill.GetFirstTray();

            if (tray == null)
                return;

            foreach (Image foodImage in tray.FoodList)
            {
                if (!foodImage.gameObject.activeInHierarchy)
                    continue;

                AddFoodToGroup(foodImage, foodGroups);
            }
        }


        // Thêm food vào group dựa theo tên sprite
        private void AddFoodToGroup(Image foodImage,Dictionary<string, List<Image>> foodGroups)
        {
            string foodName = foodImage.sprite.name;

            if (!foodGroups.ContainsKey(foodName))
            {
                foodGroups.Add(foodName, new List<Image>());
            }

            foodGroups[foodName].Add(foodImage);
        }


        // Tìm một loại food có ít nhất 3 cái
        private List<Image> FindMagnetTarget(Dictionary<string, List<Image>> foodGroups)
        {
            foreach (var group in foodGroups)
            {
                if (group.Value.Count >= 3)
                    return group.Value;
            }

            return null;
        }


        // Trừ booster + update UI
        private void UseMagnetBooster()
        {
            Pref.Magnet--;

            _magnetTxt.text = Pref.Magnet.ToString();

            AudioController.Instance.PlaySound(AudioController.Instance.Bubble);
        }


        // Chạy hiệu ứng hút 3 food
        private IEnumerator IECollectWithMagnet(List<Image> targetFoods)
        {
            _magnetFX.DOScale(Vector3.one, 0.4f);

            yield return new WaitForSeconds(0.3f);

            for (int i = 0; i < 3; i++)
            {
                PlayMagnetFoodFX(_magnetList[i],targetFoods[i]);
                yield return new WaitForSeconds(0.1f);
            }

            // Báo cho shipper món vừa hoàn thành
            Shipper.Instance.OnFoodCompleted(targetFoods[0].sprite);

            // Đợi animation hút kết thúc
            yield return new WaitForSeconds(1.5f);

            RefreshGrillsAfterMagnet();

            _magnetFX
                .DOScale(Vector3.zero, 0.5f)
                .SetEase(Ease.InBack);

            OnMinusFood();
        }


        // Hiệu ứng hút của từng food
        private void PlayMagnetFoodFX(Image dummyImage,Image foodImage)
        {
            dummyImage.sprite = foodImage.sprite;
            dummyImage.SetNativeSize();

            dummyImage.transform.position =foodImage.transform.position;

            dummyImage.color = Color.white;
            dummyImage.gameObject.SetActive(true);

            // Ẩn food thật
            foodImage.gameObject.SetActive(false);

            Vector3 middlePoint = (dummyImage.transform.position + _magnetFX.position) / 2f;

            middlePoint += new Vector3(Random.Range(-2f, 2f),Random.Range(-2f, 2f),0f);

            Vector3[] path = {dummyImage.transform.position,middlePoint,_magnetFX.position};

            Sequence sequence = DOTween.Sequence();

            sequence.Join(dummyImage.transform.DOPath(path,1.5f,PathType.CatmullRom));

            sequence.Join(dummyImage.DOColor(new Color(1f, 1f, 1f, 0.1f),1.5f));

            sequence.SetEase(Ease.OutQuad);

            sequence.OnComplete(() =>
            {
                dummyImage.gameObject.SetActive(false);
                dummyImage.transform.localScale = Vector3.one;
            });
        }


        // Kiểm tra lại trạng thái các bếp sau khi Magnet hút
        private void RefreshGrillsAfterMagnet()
        {
            foreach (GrillStation grill in _listGrills)
            {
                if (!grill.gameObject.activeInHierarchy)
                    continue;

                // Magnet có thể hút hết food của khay đầu
                grill.OnCheckFirstTray();

                // Magnet có thể hút hết food trên bếp
                grill.OnCheckPrepareTray();
            }
        }

        public void OnShuffle()
        {
            if(Pref.Shuffle >= 1)
            {
                AudioController.Instance.PlaySound(AudioController.Instance.Bubble);
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
                Pref.Shuffle--;
                _shuffleTxt.text = Pref.Shuffle.ToString();
            }          
        }

        public void OnAddMoreGrill()
        {
            if (Pref.ExtraGrill <= 0)
                return;

            AudioController.Instance.PlaySound(AudioController.Instance.Bubble);
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
            Pref.ExtraGrill--;
            _extraGrillTxt.text = Pref.ExtraGrill.ToString();
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

    

    public List<Sprite> OnShipper()
        {
            List<Sprite> curentFood = new List<Sprite>();
            for (int i = 0; i < _listGrills.Count; i++)
            {
                GrillStation grill = _listGrills[i];
                for(int j = 0; j < grill.TotalSlot.Count; j++)
                {
                    FoodSlot slot = grill.TotalSlot[j];
                    if (slot.HasFood && !curentFood.Contains(slot.GetSpriteFood))
                    {
                        curentFood.Add(slot.GetSpriteFood);
                    }                  
                }
                Trayitem tray = grill.GetFirstTray();
                if (tray != null)
                {
                    for (int k = 0; k < tray.FoodList.Count; k++)
                    {
                        Image img = tray.FoodList[k];

                        if (img.gameObject.activeInHierarchy)
                        {
                            Sprite food = img.sprite;

                            if (!curentFood.Contains(food))
                                curentFood.Add((food));
                        }
                    }
                }
            }

            //Sap xep ngau nhien
            for(int i = 0; i < curentFood.Count; i++)
            {
                int randomIndex = Random.Range(i, curentFood.Count);

                (curentFood[i], curentFood[randomIndex]) = (curentFood[randomIndex], curentFood[i]);
            }

            // lay so do an yeu cau requiredFoodCount
            while(curentFood.Count > _requiredFoodCount)
            {
                curentFood.RemoveAt(curentFood.Count - 1);
            }
            return curentFood;
        }

        private void UpdateBoosterUI()
        {
            _magnetTxt.text = Pref.Magnet.ToString();
            _shuffleTxt.text = Pref.Shuffle.ToString();
            _extraGrillTxt.text = Pref.ExtraGrill.ToString();
        }
    }

}

