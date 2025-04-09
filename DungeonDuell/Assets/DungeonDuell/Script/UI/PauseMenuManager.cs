using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace dungeonduell
{
    public class PauseMenuManager : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject pausePanel;
        public GameObject optionsPanel;
        public GameObject tutorialPanel;
        public GameObject confirmationPopup;

        [Header("Settings")]
        public float fadeDuration = 0.25f;
        public GameObject defaultSelectedButton;

        private CanvasGroup pauseGroup;
        private bool isPaused = false;
        private InputAction pauseAction;

        void OnEnable()
        {
            pauseAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/escape");
            pauseAction.AddBinding("<Gamepad>/start");
            pauseAction.performed += OnPausePerformed;
            pauseAction.Enable();
        }

        void OnDisable()
        {
            pauseAction.performed -= OnPausePerformed;
            pauseAction.Disable();
        }

        void Start()
        {
            pauseGroup = pausePanel.GetComponent<CanvasGroup>();
            if (pauseGroup == null)
            {
                pauseGroup = pausePanel.AddComponent<CanvasGroup>();
            }

            pausePanel.SetActive(false);
            optionsPanel.SetActive(false);
            tutorialPanel.SetActive(false);
            confirmationPopup.SetActive(false);
        }

        private void OnPausePerformed(InputAction.CallbackContext ctx)
        {
            Debug.Log("[PauseMenuManager] Pause input received");
            if (!isPaused) OpenPauseMenu();
            else ResumeGame();
        }

        public void OpenPauseMenu()
        {
            isPaused = true;
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            pausePanel.transform.localScale = Vector3.zero;
            pauseGroup.alpha = 0;

            pausePanel.transform.DOScale(1, fadeDuration).SetEase(Ease.OutBack).SetUpdate(true);
            pauseGroup.DOFade(1, fadeDuration).SetUpdate(true).OnComplete(() =>
            {
                if (defaultSelectedButton != null && EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
                }
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
            });
        }

        public void OpenTutorial()
        {
            tutorialPanel.SetActive(true);
            RectTransform rect = tutorialPanel.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, -800);
            rect.DOAnchorPosY(0, fadeDuration).SetEase(Ease.OutCubic).SetUpdate(true);
        }

        public void CloseTutorial()
        {
            RectTransform rect = tutorialPanel.GetComponent<RectTransform>();
            rect.DOAnchorPosY(-800, fadeDuration).SetEase(Ease.InCubic).SetUpdate(true).OnComplete(() =>
            {
                tutorialPanel.SetActive(false);
            });
        }

        public void ConfirmGiveUp()
        {
            confirmationPopup.SetActive(true);
            confirmationPopup.transform.localScale = Vector3.zero;
            confirmationPopup.transform.DOScale(1, fadeDuration).SetEase(Ease.OutBack).SetUpdate(true);
        }

        public void CancelGiveUp()
        {
            confirmationPopup.transform.DOScale(0, fadeDuration).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
            {
                confirmationPopup.SetActive(false);
            });
        }

        public void GiveUpConfirmed()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Titlescreen");
        }
    }
}
