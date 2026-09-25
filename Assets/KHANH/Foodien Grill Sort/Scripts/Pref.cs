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

        public static int Coin
        {
            set => PlayerPrefs.SetInt("Coin", value);
            get => PlayerPrefs.GetInt("Coin", 0);
        }

        public static int Heart
        {
            set => PlayerPrefs.SetInt("Heart", value);
            get => PlayerPrefs.GetInt("Heart", 5);
        }
    }
}

