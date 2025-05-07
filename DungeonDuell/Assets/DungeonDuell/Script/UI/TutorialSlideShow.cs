using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using DG.Tweening;
using MoreMountains.TopDownEngine;
using System;

namespace dungeonduell
{
    public class TutorialSlideShow : MonoBehaviour
    {
        [System.Serializable]
        public class TutorialPage
        {
            public LocalizedString localizedTitle;
            public LocalizedString localizedDescription;
            public LocalizedSprite localizedImage;
        }

        [Header("UI References")]
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public Image illustrationImage;
        public TextMeshProUGUI progressText;
        public Image skipProgressBar;

        [Header("Tutorial Pages")]
        public TutorialPage[] pages;

        [Header("Skip Settings")]
        public float skipHoldDuration = 2f;

        private int currentPageIndex = 0;
        private DungeonPhaseInput inputActions;

        private float skipHoldTime = 0f;
        private bool isSkipPressed = false;
        private bool isTransitioning = false;

        private void Awake()
        {
            inputActions = new DungeonPhaseInput();

            inputActions.CardPhase.RotateR.performed += ctx => NextPage();
            inputActions.CardPhase.RotateL.performed += ctx => PreviousPage();
            inputActions.CardPhase.Pause.started += ctx => isSkipPressed = true;
            inputActions.CardPhase.Pause.canceled += ctx =>
            {
                isSkipPressed = false;
                skipHoldTime = 0f;
                UpdateSkipBar(0f);
            };

        }

        private void OnEnable()
        {
            inputActions.Enable();
            ShowPage(currentPageIndex, instant: true);
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        private void Update()
        {
            if (isSkipPressed)
            {
                skipHoldTime += Time.unscaledDeltaTime;
                UpdateSkipBar(skipHoldTime / skipHoldDuration);

                if (skipHoldTime >= skipHoldDuration)
                {
                    CloseTutorial();
                }
            }
        }

        void UpdateSkipBar(float progress)
        {
            if (skipProgressBar != null)
                skipProgressBar.fillAmount = Mathf.Clamp01(progress);
        }

        public void NextPage()
        {
            if (isTransitioning || currentPageIndex >= pages.Length - 1) return;
            currentPageIndex++;
            ShowPage(currentPageIndex);
        }

        public void PreviousPage()
        {
            if (isTransitioning || currentPageIndex <= 0) return;
            currentPageIndex--;
            ShowPage(currentPageIndex);
        }

        void ShowPage(int index, bool instant = false)
        {
            if (pages.Length == 0 || index >= pages.Length) return;

            if (instant)
            {
                ApplyPage(index);
                canvasGroup.alpha = 1;
            }
            else
            {
                isTransitioning = true;
                canvasGroup.DOFade(0, 0.25f).OnComplete(() =>
                {
                    ApplyPage(index);
                    canvasGroup.DOFade(1, 0.25f).OnComplete(() =>
                    {
                        isTransitioning = false;
                    });
                });
            }

            UpdateProgressDisplay();
        }

        void ApplyPage(int index)
        {
            var page = pages[index];

            page.localizedTitle.StringChanged += value => titleText.text = value;
            page.localizedDescription.StringChanged += value => descriptionText.text = value;
            page.localizedTitle.RefreshString();
            page.localizedDescription.RefreshString();

            if (illustrationImage != null && page.localizedImage != null)
            {
                page.localizedImage.LoadAssetAsync().Completed += handle =>
                {
                    illustrationImage.sprite = handle.Result;
                    illustrationImage.enabled = (handle.Result != null);
                };
            }
        }

        void UpdateProgressDisplay()
        {
            if (progressText != null)
                progressText.text = $"{currentPageIndex + 1} / {pages.Length}";
        }

        void CloseTutorial()
        {
            canvasGroup.DOFade(0, 0.25f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                ResetTutorial();

            });
        }

        void ResetTutorial()
        {
            currentPageIndex = 0;
            skipHoldTime = 0f;
            isSkipPressed = false;
            isTransitioning = false;

            UpdateSkipBar(0f);
            ShowPage(currentPageIndex, instant: true);
        }
    }
}
