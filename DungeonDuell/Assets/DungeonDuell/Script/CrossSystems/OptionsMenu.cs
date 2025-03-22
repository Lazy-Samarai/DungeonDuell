using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using TMPro;
using UnityEngine.Audio;

namespace dungeonduell
{
    public class OptionsMenu : MonoBehaviour
    {
        public GameObject optionsPanel;
        public AudioMixer audioMixer;
        public Slider audioSlider;
        public Toggle muteToggle;
        public Toggle fullscreenToggle;
        public TMP_Dropdown resolutionDropdown;

        private CanvasGroup canvasGroup;
        private Resolution[] resolutions;
        private OptionDataManager dataManager;


        void Start()
        {
            canvasGroup = optionsPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = optionsPanel.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0;
            optionsPanel.SetActive(false);

            dataManager = FindObjectOfType<OptionDataManager>();
            SetupResolutionDropdown();
            LoadSettings();
        }

        public void OpenOptions()
        {
            optionsPanel.SetActive(true);
            canvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutQuad);
        }

        public void CloseOptions()
        {
            canvasGroup.DOFade(0, 0.5f).SetEase(Ease.InQuad).OnComplete(() => optionsPanel.SetActive(false));
        }


        public void SetMasterVolume(float volume)
        {

            if (dataManager != null)
            {
                dataManager.SetVolume(volume);
            }
            else
            {
                Debug.Log("Datamanager fehlt");
            }
        }

        public void SetMusicVolume(float volume)
        {
            if (dataManager != null)
            {
                dataManager.SetMusicVolume(volume);
            }
        }
        public void SetSFXVolume(float volume)
        {
            if (dataManager != null)
            {
                dataManager.SetSFXVolume(volume);
            }
        }

        public void MuteToggle(bool muted)
        {
            if (dataManager != null)
            {
                dataManager.MuteToggle(muted);
            }
        }

        void SetupResolutionDropdown()
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolutions[i].width + "x" + resolutions[i].height));
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                    Debug.Log(currentResolutionIndex);
                }
            }

            
            if (dataManager != null)
            {
                resolutionDropdown.value = dataManager.ResolutionIndex;
            }
            resolutionDropdown.RefreshShownValue();
        }

        public void SetFullscreen(bool isFullscreen)
        {
            
            if (dataManager != null)
            {
                dataManager.SetFullscreen(isFullscreen);
            }
        }

        public void SetResolution(int resolutionIndex)
        {
            
            if (dataManager != null)
            {
                dataManager.SetResolution(resolutionIndex);
            }
        }

        void LoadSettings()
        {
            
            if (dataManager != null)
            {
                audioSlider.value = dataManager.Volume;
                fullscreenToggle.isOn = dataManager.IsFullscreen;
                resolutionDropdown.value = dataManager.ResolutionIndex;
                muteToggle.isOn = dataManager.isMuted;
            }
        }
    }
}
