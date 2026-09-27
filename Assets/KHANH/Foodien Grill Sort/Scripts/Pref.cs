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
            get => PlayerPrefs.GetInt("Coin", 100);
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
            get => PlayerPrefs.GetFloat("SOUND_VOL__PREF", 0.8f);
        }

        public static int Magnet
        {
            set => PlayerPrefs.SetInt("Magnet", value);
            get => PlayerPrefs.GetInt("Magnet", 1);
        }

        public static int Shuffle
        {
            set => PlayerPrefs.SetInt("Shuffle", value);
            get => PlayerPrefs.GetInt("Shuffle", 1);
        }

        public static int ExtraGrill
        {
            set => PlayerPrefs.SetInt("ExtraGrill", value);
            get => PlayerPrefs.GetInt("ExtraGrill", 1);
        }

        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }

        public static bool IsSoundStopping
        {
            set => SetBool("is_sound_stopping", value);
            get => GetBool("is_sound_stopping", false);
        }

        public static bool IsMusicStopping
        {
            set => SetBool("is_music_stopping", value);
            get => GetBool("is_music_stopping", false);
        }
    }
}

