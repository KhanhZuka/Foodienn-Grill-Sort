using UnityEngine;

namespace KHANH.FoodienGrillSort
{
    public static class Pref
    {
        public static int CurrentLevel
        {
            set => PlayerPrefs.SetInt("CurrentLevel", value);
            get => PlayerPrefs.GetInt("CurrentLevel", 0);
        }
    }
}

