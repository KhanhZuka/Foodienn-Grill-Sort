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
            get => PlayerPrefs.GetInt("Coin", 40);
        }

        public static int Heart
        {
            set => PlayerPrefs.SetInt("Heart", value);
            get => PlayerPrefs.GetInt("Heart", 5);
        }

        public static float musicVol
        {
            set => PlayerPrefs.SetFloat("MUSIC_VOL_PREF", value);
            get => PlayerPrefs.GetFloat("MUSIC_VOL_PREF", 0.3f);
        }

        public static float soundVol
        {
            set => PlayerPrefs.SetFloat("SOUND_VOL__PREF", value);
            get => PlayerPrefs.GetFloat("SOUND_VOL__PREF", 1f);
        }
    }
}

