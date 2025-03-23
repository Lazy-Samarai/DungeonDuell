using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Audio;

namespace dungeonduell
{
    public class OptionDataManager : MonoBehaviour
    {
        public static OptionDataManager Instance = null;
        // Neue Settings für das Optionsmenü
        public float Volume = 1f;
        public AudioMixer audioMixer;

        public bool IsFullscreen = true;
        public bool isMuted = true;
        public int ResolutionIndex = 0;
        const string masterVolume = "MasterVolume";
        const string musicVolume = "MusicVolume";
        const string sfxVolume = "SFXVolume";
        private int volumeMultiplier = 20;

        private void Awake()
        {
            // If there is not already an instance of SoundManager, set it to this.
            if (Instance == null)
            {
                Instance = this;
            }
            //If an instance already exists, destroy whatever this object is to enforce the singleton.
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
            DontDestroyOnLoad(gameObject);
        }

        public void SetVolume(float volume)
        {
            Volume = volume;
            if (audioMixer != null)
            {
                audioMixer.SetFloat(masterVolume, Mathf.Log10(volume) * volumeMultiplier);
            }
        }

        public void SetMusicVolume(float volume)
        {
            Volume = volume;
            audioMixer.SetFloat(musicVolume, Mathf.Log10(volume) * volumeMultiplier);
        }
        public void SetSFXVolume(float volume)
        {
            Volume = volume;
            audioMixer.SetFloat(sfxVolume, Mathf.Log10(volume) * volumeMultiplier);
        }
        public void MuteToggle(bool muted)
        {
            isMuted = muted;
            if (isMuted)
            {
                AudioListener.volume = 0;
            }
            else
            {
                AudioListener.volume = 1;
            }
        }

        public void SetFullscreen(bool isFullscreen)
        {
            IsFullscreen = isFullscreen;
            Screen.fullScreen = isFullscreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            ResolutionIndex = resolutionIndex;
            Resolution resolution = Screen.resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }
}
