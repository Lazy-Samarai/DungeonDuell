using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace dungeonduell
{
    public class OptionDataManager : MonoBehaviour
    {
        private const string MasterVolume = "MasterVolume";
        private const string MusicVolume = "MusicVolume";
        private const string SfxVolume = "SFXVolume";

        public static OptionDataManager Instance;

        // Neue Settings f�r das Optionsmen�
        [FormerlySerializedAs("Volume")] public float volume = 1f;
        public AudioMixer audioMixer;

        [FormerlySerializedAs("IsFullscreen")] public bool isFullscreen = true;
        public bool isMuted = true;

        [FormerlySerializedAs("ResolutionIndex")]
        public int resolutionIndex;

        public string selectedLanguageCode = "en"; // Standard auf Englisch


        private readonly int _volumeMultiplier = 20;


        private void Awake()
        {
            // If there is not already an instance of SoundManager, set it to this.
            if (Instance == null)
                Instance = this;
            //If an instance already exists, destroy whatever this object is to enforce the singleton.
            else if (Instance != this) Destroy(gameObject);

            //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            ApplyLanguageSetting();
        }

        public void SetVolume(float volume)
        {
            this.volume = volume;
            if (audioMixer != null) audioMixer.SetFloat(MasterVolume, Mathf.Log10(volume) * _volumeMultiplier);
        }

        public void SetMusicVolume(float volume)
        {
            this.volume = volume;
            audioMixer.SetFloat(MusicVolume, Mathf.Log10(volume) * _volumeMultiplier);
        }

        public void SetSfxVolume(float volume)
        {
            this.volume = volume;
            audioMixer.SetFloat(SfxVolume, Mathf.Log10(volume) * _volumeMultiplier);
        }

        public void MuteToggle(bool muted)
        {
            isMuted = muted;
            if (isMuted)
                AudioListener.volume = 0;
            else
                AudioListener.volume = 1;
        }

        public void SetFullscreen(bool isFullscreen)
        {
            this.isFullscreen = isFullscreen;
            Screen.fullScreen = isFullscreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            this.resolutionIndex = resolutionIndex;
            var resolution = Screen.resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetLanguage(string localeCode)
        {
            selectedLanguageCode = localeCode;
            var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
            if (locale != null)
            {
                LocalizationSettings.SelectedLocale = locale;
            }
        }

        private void ApplyLanguageSetting()
        {
            if (!string.IsNullOrEmpty(selectedLanguageCode))
            {
                var locale = LocalizationSettings.AvailableLocales.GetLocale(selectedLanguageCode);
                if (locale != null)
                {
                    LocalizationSettings.SelectedLocale = locale;
                }
            }
        }
    }
}