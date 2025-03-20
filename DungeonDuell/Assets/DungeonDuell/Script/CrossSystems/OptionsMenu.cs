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
        public Slider volumeSlider;
        public AudioMixer audioMixer;
        public Toggle fullscreenToggle;
        public TMP_Dropdown resolutionDropdown;

        private CanvasGroup canvasGroup;
        private Resolution[] resolutions;
        private PlayerDataManager dataManager;


        void Start()
        {
            canvasGroup = optionsPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = optionsPanel.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0;
            optionsPanel.SetActive(false);

            dataManager = FindObjectOfType<PlayerDataManager>();
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
                }
            }

            
            if (dataManager != null)
            {
                resolutionDropdown.value = dataManager.ResolutionIndex;
            }
            resolutionDropdown.RefreshShownValue();
        }

        public void SetMasterVolume(float volume)
        {
            
            if (dataManager != null)
            {
                dataManager.SetVolume(volume);
            }
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
                volumeSlider.value = dataManager.Volume;
                fullscreenToggle.isOn = dataManager.IsFullscreen;
                resolutionDropdown.value = dataManager.ResolutionIndex;
            }
        }
    }
}
