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

        private DungeonPhaseInput controls;
        private bool isPaused;

        private CanvasGroup pauseGroup;
        private GameObject previousSelected;

        private void Awake()
        {
            controls = new DungeonPhaseInput();
            controls.CardPhase.Pause.performed += ctx => TogglePause();
        }

        private void Start()
        {
            pauseGroup = pausePanel.GetComponent<CanvasGroup>();
            if (pauseGroup == null) pauseGroup = pausePanel.AddComponent<CanvasGroup>();

            pausePanel.SetActive(false);
            optionsPanel.SetActive(false);
            tutorialPanel.SetActive(false);
            confirmationPopup.SetActive(false);
        }

        private void OnEnable()
        {
            controls.CardPhase.Enable();
        }

        private void OnDisable()
        {
            controls.CardPhase.Disable();
        }

        private void TogglePause()
        {
            Debug.Log("Pause Input");
            if (!isPaused) OpenPauseMenu();
            else ResumeGame();
        }

        public void OpenPauseMenu()
        {
            isPaused = true;
            Time.timeScale = 1f;
            pausePanel.SetActive(true);
            pausePanel.transform.localScale = Vector3.zero;
            pauseGroup.alpha = 0;

            if (EventSystem.current != null)
            {
                previousSelected = EventSystem.current.currentSelectedGameObject;
                EventSystem.current.SetSelectedGameObject(null);
            }

            pausePanel.transform.DOScale(1, fadeDuration).SetEase(Ease.OutBack).SetUpdate(true);
            pauseGroup.DOFade(1, fadeDuration).SetUpdate(true).OnComplete(() =>
            {
                if (defaultSelectedButton != null && EventSystem.current != null)
                    EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
            });
        }

        public void ResumeGame()
        {
            pausePanel.transform.DOScale(0, fadeDuration).SetEase(Ease.InBack).SetUpdate(true);
            pauseGroup.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
            {
                pausePanel.SetActive(false);
                Time.timeScale = 1f;
                isPaused = false;

                if (EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    if (previousSelected != null) EventSystem.current.SetSelectedGameObject(previousSelected);
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