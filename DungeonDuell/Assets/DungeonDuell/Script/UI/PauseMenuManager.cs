using DG.Tweening;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace dungeonduell
{
    public class PauseMenuManager : MonoBehaviour
    {
        [Header("Panels & Buttons")] public GameObject pausePanel;

        public GameObject defaultSelectedButton;
        public GameObject optionsPanel;
        public GameObject tutorialPanel;
        public GameObject tutorialSelectedButton;
        public GameObject confirmationPopup;
        public GameObject confirmSelectedButton;

        [Header("Settings")] public float fadeDuration = 0.25f;

        private DungeonPhaseInput _controls;
        private bool _isPaused;

        private CanvasGroup _pauseGroup;
        private GameObject _previousSelected;

        private void Awake()
        {
            _controls = new DungeonPhaseInput();
            _controls.CardPhase.Pause.performed += ctx => TogglePause();
        }

        private void Start()
        {
            _pauseGroup = pausePanel.GetComponent<CanvasGroup>();
            if (_pauseGroup == null) _pauseGroup = pausePanel.AddComponent<CanvasGroup>();

            pausePanel.SetActive(false);
            optionsPanel.SetActive(false);
            tutorialPanel.SetActive(false);
            confirmationPopup.SetActive(false);
        }

        private void OnEnable()
        {
            _controls.CardPhase.Enable();
        }

        private void OnDisable()
        {
            _controls.CardPhase.Disable();
        }

        private void TogglePause()
        {
            Debug.Log("Pause Input");
            if (!_isPaused) OpenPauseMenu();
            else ResumeGame();
        }

        public void OpenPauseMenu()
        {
            _isPaused = true;
            Time.timeScale = 1f;
            pausePanel.SetActive(true);
            pausePanel.transform.localScale = Vector3.zero;
            _pauseGroup.alpha = 0;

            if (EventSystem.current != null)
            {
                _previousSelected = EventSystem.current.currentSelectedGameObject;
                EventSystem.current.SetSelectedGameObject(null);
            }

            pausePanel.transform.DOScale(1, fadeDuration).SetEase(Ease.OutBack).SetUpdate(true);
            _pauseGroup.DOFade(1, fadeDuration).SetUpdate(true).OnComplete(() =>
            {
                if (defaultSelectedButton != null && EventSystem.current != null)
                    EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
            });
        }

        public void ResumeGame()
        {
            pausePanel.transform.DOScale(0, fadeDuration).SetEase(Ease.InBack).SetUpdate(true);
            _pauseGroup.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
            {
                pausePanel.SetActive(false);
                Time.timeScale = 1f;
                _isPaused = false;

                if (EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    if (_previousSelected != null) EventSystem.current.SetSelectedGameObject(_previousSelected);
                }
            });
        }

        public void OpenTutorial()
        {
            tutorialPanel.SetActive(true);
            var rect = tutorialPanel.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, -800);
            rect.DOAnchorPosY(0, fadeDuration).SetEase(Ease.OutCubic).SetUpdate(true).OnComplete(() =>
            {
                if (tutorialSelectedButton != null && EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(tutorialSelectedButton);
                }
            });
        }

        public void CloseTutorial()
        {
            var rect = tutorialPanel.GetComponent<RectTransform>();
            rect.DOAnchorPosY(-800, fadeDuration).SetEase(Ease.InCubic).SetUpdate(true).OnComplete(() =>
            {
                tutorialPanel.SetActive(false);
                if (defaultSelectedButton != null && EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
                }
            });
        }

        public void ConfirmGiveUp()
        {
            confirmationPopup.SetActive(true);
            confirmationPopup.transform.localScale = Vector3.zero;
            confirmationPopup.transform.DOScale(1, fadeDuration).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() =>
            {
                if (confirmSelectedButton != null && EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(confirmSelectedButton);
                }
            });
        }

        public void CancelGiveUp()
        {
            confirmationPopup.transform.DOScale(0, fadeDuration).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
            {
                confirmationPopup.SetActive(false);
                if (defaultSelectedButton != null && EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
                }
            });
        }

        public void GiveUpConfirmed()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Titlescreen");
        }
    }
}