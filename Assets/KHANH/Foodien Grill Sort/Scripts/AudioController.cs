using UnityEngine;
using UnityEngine.UI;

namespace KHANH.FoodienGrillSort
{
    public class AudioController : MonoBehaviour
    {
        private static AudioController _instance;
        public static AudioController Instance => _instance;

        [Header("Main Setting")]
        [Range(0f, 1f)]
        public float MusicVol = 0.3f;

        [Range(0f, 1f)]
        public float SoundVol = 1f;

        [Header("Audio Source")]
        public AudioSource MusicAus; // Nhạc nền
        public AudioSource SoundAus; // Âm thanh hiệu ứng

        [Header("Music and Sound in GamePlay")]
        public AudioClip Bubble;
        public AudioClip GrilledMeat;
        public AudioClip MergeFood;
        public AudioClip LevelComplete;
        public AudioClip Bip;
        public AudioClip ThankYou;
        public AudioClip LoseGame;

        public AudioClip[] Bgms;
        public Image BanMusicImg;
        public Image BanSoundImg;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            if (MusicAus == null || SoundAus == null)
                return;

            MusicVol = Pref.musicVol;
            SoundVol = Pref.soundVol;

            MusicAus.volume =
                Pref.IsMusicStopping ? 0f : MusicVol;

            SoundAus.volume =
                Pref.IsSoundStopping ? 0f : SoundVol;
        }


        public void PlaySound(AudioClip sound, AudioSource aus = null)
        {
            if (Pref.IsSoundStopping)
                return;

            if (aus == null)
                aus = SoundAus;

            if (aus == null || sound == null)
                return;

            aus.PlayOneShot(sound, SoundVol);
        }

        public void StopSound()
        {
            Pref.IsSoundStopping = !Pref.IsSoundStopping;
            BanSoundImg.gameObject.SetActive(Pref.IsSoundStopping);

            if (Pref.IsSoundStopping)
            {
                if (SoundAus != null)
                {
                    SoundAus.Stop();
                    SoundAus.volume = 0f;
                }
            }
            else
            {
                if (SoundAus != null)
                {
                    SoundAus.volume = Pref.soundVol;
                }
            }
        }


        public void PlayMusic(AudioClip[] musics,int index,bool isLoop = true)
        {
            if (MusicAus == null)
                return;

            if (musics == null || musics.Length <= 0)
                return;

            if (index < 0 || index >= musics.Length)
                return;

            if (musics[index] == null)
                return;

            MusicAus.clip = musics[index];
            MusicAus.loop = isLoop;

            MusicAus.volume =
                Pref.IsMusicStopping ? 0f : MusicVol;

            MusicAus.Play();
        }

        public void PlayMusic(AudioClip music,bool isLoop = true)
        {
            if (MusicAus == null || music == null)
                return;

            MusicAus.clip = music;
            MusicAus.loop = isLoop;

            MusicAus.volume =
                Pref.IsMusicStopping ? 0f : MusicVol;

            MusicAus.Play();
        }

        public void StopMusic()
        {
            Pref.IsMusicStopping = !Pref.IsMusicStopping;
            BanMusicImg.gameObject.SetActive(Pref.IsMusicStopping);

            if (MusicAus == null)
                return;

            if (Pref.IsMusicStopping)
            {
                MusicAus.volume = 0f;
            }
            else
            {
                MusicAus.volume = Pref.musicVol;
            }
        }

        public void SetMusicVolume(float vol)
        {
            MusicVol = Mathf.Clamp01(vol);

            Pref.musicVol = MusicVol;

            if (MusicAus != null && !Pref.IsMusicStopping)
            {
                MusicAus.volume = MusicVol;
            }
        }


        public void SetSoundVolume(float vol)
        {
            SoundVol = Mathf.Clamp01(vol);

            Pref.soundVol = SoundVol;

            if (SoundAus != null && !Pref.IsSoundStopping)
            {
                SoundAus.volume = SoundVol;
            }
        }

        public void PauseMusic()
        {
            if (MusicAus == null)
                return;

            MusicAus.Pause();
        }

        public void ResumeMusic()
        {
            if (MusicAus == null)
                return;

            if (Pref.IsMusicStopping)
                return;

            MusicAus.UnPause();
        }

    }
}