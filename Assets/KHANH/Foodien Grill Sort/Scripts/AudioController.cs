using UnityEngine;

namespace KHANH.FoodienGrillSort
{
    public class AudioController : MonoBehaviour
    {
        private static AudioController _instance;
        public static AudioController Instance => _instance;

        [Header("Main Setting:")]
        [Range(0f, 1f)]
        public float musicVol = 0.3f;
        [Range(0f, 1f)]
        public float soundVol = 1f;

        public AudioSource musicAus; //nhac nen
        public AudioSource soundAus; //tieng dong trong game

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
            if (musicAus == null || soundAus == null) return;

            musicVol = Pref.musicVol;
            soundVol = Pref.soundVol;

            musicAus.volume = musicVol;
            soundAus.volume = soundVol;
        }

        public void PlaySound(AudioClip[] sounds, AudioSource aus = null)
        {
            if (!aus)
                aus = soundAus;

            if (aus == null) return;

            if (sounds == null || sounds.Length <= 0) return;

            int randIdx = Random.Range(0, sounds.Length);
            if (sounds[randIdx])
                aus.PlayOneShot(sounds[randIdx], soundVol); // phat 1 am thanh hieu ung trong game
        }

        public void PlaySound(AudioClip sound, AudioSource aus = null)
        {
            if (!aus)
                aus = soundAus;

            if (aus == null) return;

            if (sound)
                aus.PlayOneShot(sound, soundVol);
        }

        public void PlayMusic(AudioClip[] musics, int index,bool isLoop = true)
        {
            if (musicAus == null || musics == null || musics.Length <= 0) return;

            //int randIdx = Random.Range(0, musics.Length);

            if (musics[index])
            {
                musicAus.clip = musics[index];
                musicAus.loop = isLoop;
                musicAus.volume = musicVol;
                musicAus.Play();
            }
        }

        public void PlayMusic(AudioClip music, bool isLoop = true)
        {
            if (musicAus == null || music == null) return;

            musicAus.clip = music;
            musicAus.loop = isLoop;
            musicAus.volume = musicVol;
            musicAus.Play();
        }

        public void SetMusicVolume(float vol)
        {
            if (musicAus == null) return;

            musicAus.volume = vol;
        }

        public void StopMusic()
        {
            if (musicAus == null) return;

            musicAus.Stop();
        }

        //public void PlayBmg()
        //{
        //    PlayMusic(bgms);
        //}
    }
}
