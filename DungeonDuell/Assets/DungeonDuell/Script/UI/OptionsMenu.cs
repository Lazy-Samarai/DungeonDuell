using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace dungeonduell
{
    public class OptionsMenu : MonoBehaviour
    {
        public GameObject optionsPanel;
        public GameObject optionSelectedButton;
        public GameObject previousSelected;
        public AudioMixer audioMixer;
        public Slider audioSlider;
        public Toggle muteToggle;
        public Toggle fullscreenToggle;
        public TMP_Dropdown resolutionDropdown;

        public float fadeDuration = 0.25f;

        private CanvasGroup _canvasGroup;
        private OptionDataManager _dataManager;
        private Resolution[] _resolutions;


        private void Start()
        {
            _canvasGroup = optionsPanel.GetComponent<CanvasGroup>();
            if (_canvasGroup == null) _canvasGroup = optionsPanel.AddComponent<CanvasGroup>();

            _canvasGroup.alpha = 0;
            optionsPanel.SetActive(false);

            _dataManager = FindFirstObjectByType<OptionDataManager>();
            SetupResolutionDropdown();
            LoadSettings();
        }

        public void OpenOptions()
        {
            optionsPanel.SetActive(true);
            optionsPanel.transform.localScale = Vector3.zero;
            _canvasGroup.alpha = 0;
            optionsPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
            _canvasGroup.DOFade(1, fadeDuration).SetUpdate(true).OnComplete(() =>
            {
                if (optionSelectedButton != null && EventSystem.current != null)
                    EventSystem.current.SetSelectedGameObject(optionSelectedButton);
            });
        }

        public void CloseOptions()
        {
            optionsPanel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).SetUpdate(true);
            _canvasGroup.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
            {
                optionsPanel.SetActive(false);
                if (EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    if (previousSelected != null) EventSystem.current.SetSelectedGameObject(previousSelected);
                }
            });
        }


        public void SetMasterVolume(float volume)
        {
            if (_dataManager != null)
                _dataManager.SetVolume(volume);
            else
                Debug.Log("Datamanager fehlt");
        }

        public void SetMusicVolume(float volume)
        {
            if (_dataManager != null) _dataManager.SetMusicVolume(volume);
        }

        public void SetSfxVolume(float volume)
        {
            if (_dataManager != null) _dataManager.SetSfxVolume(volume);
        }

        public void MuteToggle(bool muted)
        {
            if (_dataManager != null) _dataManager.MuteToggle(muted);
        }

        private void SetupResolutionDropdown()
        {
            _resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            var currentResolutionIndex = 0;
            for (var i = 0; i < _resolutions.Length; i++)
            {
                resolutionDropdown.options.Add(
                    new TMP_Dropdown.OptionData(_resolutions[i].width + "x" + _resolutions[i].height));
                if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                    Debug.Log(currentResolutionIndex);
                }
            }

            if (_dataManager != null) resolutionDropdown.value = _dataManager.resolutionIndex;

            resolutionDropdown.RefreshShownValue();
        }

        public void SetFullscreen(bool isFullscreen)
        {
            if (_dataManager != null) _dataManager.SetFullscreen(isFullscreen);
        }

        public void SetResolution(int resolutionIndex)
        {
            if (_dataManager != null) _dataManager.SetResolution(resolutionIndex);
        }

        private void LoadSettings()
        {
            if (_dataManager != null)
            {
                audioSlider.value = _dataManager.volume;
                fullscreenToggle.isOn = _dataManager.isFullscreen;
                resolutionDropdown.value = _dataManager.resolutionIndex;
                muteToggle.isOn = _dataManager.isMuted;
            }
        }
    }
}