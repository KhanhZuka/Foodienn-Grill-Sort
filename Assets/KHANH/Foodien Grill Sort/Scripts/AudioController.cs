using UnityEngine;

namespace KHANH.FoodienGrillSort
{
    public class AudioController : MonoBehaviour
    {
        private static AudioController _instance;
        public static AudioController Instance => _instance;

        [Header("Main Setting")]
        [Range(0f, 1f)]
        public float musicVol = 0.3f;

        [Range(0f, 1f)]
        public float soundVol = 1f;

        [Header("Audio Source")]
        public AudioSource musicAus; // Nhạc nền
        public AudioSource soundAus; // Âm thanh hiệu ứng

        [Header("Music and Sound in GamePlay")]
        public AudioClip Bubble;
        public AudioClip GrilledMeat;
        public AudioClip MergeFood;
        public AudioClip LevelComplete;
        public AudioClip Bip;
        public AudioClip ThankYou;
        public AudioClip LoseGame;

        public AudioClip[] bgms;


        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            if (musicAus == null || soundAus == null)
                return;

            musicVol = Pref.musicVol;
            soundVol = Pref.soundVol;

            musicAus.volume =
                Pref.IsMusicStopping ? 0f : musicVol;

            soundAus.volume =
                Pref.IsSoundStopping ? 0f : soundVol;
        }



        public void PlaySound(AudioClip[] sounds, AudioSource aus = null)
        {
            if (Pref.IsSoundStopping)
                return;

            if (aus == null)
                aus = soundAus;

            if (aus == null)
                return;

            if (sounds == null || sounds.Length <= 0)
                return;

            int randIdx = Random.Range(0, sounds.Length);

            AudioClip sound = sounds[randIdx];

            if (sound != null)
            {
                aus.PlayOneShot(sound, soundVol);
            }
        }

        public void PlaySound(AudioClip sound, AudioSource aus = null)
        {
            if (Pref.IsSoundStopping)
                return;

            if (aus == null)
                aus = soundAus;

            if (aus == null || sound == null)
                return;

            aus.PlayOneShot(sound, soundVol);
        }

        public void StopSound()
        {
            Pref.IsSoundStopping = !Pref.IsSoundStopping;

            if (Pref.IsSoundStopping)
            {
                if (soundAus != null)
                {
                    soundAus.Stop();
                    soundAus.volume = 0f;
                }
            }
            else
            {
                if (soundAus != null)
                {
                    soundAus.volume = Pref.soundVol;
                }
            }
        }


        public void PlayMusic(AudioClip[] musics,int index,bool isLoop = true)
        {
            if (musicAus == null)
                return;

            if (musics == null || musics.Length <= 0)
                return;

            if (index < 0 || index >= musics.Length)
                return;

            if (musics[index] == null)
                return;

            musicAus.clip = musics[index];
            musicAus.loop = isLoop;

            musicAus.volume =
                Pref.IsMusicStopping ? 0f : musicVol;

            musicAus.Play();
        }

        public void PlayMusic(AudioClip music,bool isLoop = true)
        {
            if (musicAus == null || music == null)
                return;

            musicAus.clip = music;
            musicAus.loop = isLoop;

            musicAus.volume =
                Pref.IsMusicStopping ? 0f : musicVol;

            musicAus.Play();
        }

        public void StopMusic()
        {
            Pref.IsMusicStopping = !Pref.IsMusicStopping;

            if (musicAus == null)
                return;

            if (Pref.IsMusicStopping)
            {
                musicAus.volume = 0f;
            }
            else
            {
                musicAus.volume = Pref.musicVol;
            }
        }

        public void SetMusicVolume(float vol)
        {
            musicVol = Mathf.Clamp01(vol);

            Pref.musicVol = musicVol;

            if (musicAus != null && !Pref.IsMusicStopping)
            {
                musicAus.volume = musicVol;
            }
        }


        public void SetSoundVolume(float vol)
        {
            soundVol = Mathf.Clamp01(vol);

            Pref.soundVol = soundVol;

            if (soundAus != null && !Pref.IsSoundStopping)
            {
                soundAus.volume = soundVol;
            }
        }



        public void StopOneMusic()
        {
            if (musicAus == null)
                return;

            musicAus.Stop();
        }
    }
}