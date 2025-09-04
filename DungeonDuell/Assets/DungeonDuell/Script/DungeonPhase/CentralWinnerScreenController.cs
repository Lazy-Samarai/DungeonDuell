using System.Collections;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace dungeonduell
{
    public class CentralWinnerScreenController : MonoBehaviour
    {
        [Header("Visual References")] [SerializeField]
        private SkeletonGraphic player1Visual;

        [SerializeField] private SkeletonGraphic player2Visual;

        [Header("Winner Panel")] [SerializeField]
        private GameObject winnerPanel;

        [SerializeField] private GameObject buttonsContainer;

        [Header("Animation Settings")] [SerializeField]
        private float delayBeforeButtons = 2f;

        [SerializeField] private float scaleDuration = 0.5f;
        [SerializeField] private float winnerScale = 1.2f;
        [SerializeField] private float loserScale = 0.8f;
        [SerializeField] private Color loserColor = Color.gray;
        [SerializeField] private Color normalColor = Color.white;

        [SerializeField] private SequenceMang sequenceMang;

        const string WinnerAnimation = "WinIdle";


        private void Start()
        {
            winnerPanel.SetActive(false);
            buttonsContainer.SetActive(false);

            if (sequenceMang == null)
            {
                sequenceMang = FindFirstObjectByType<SequenceMang>();
                if (sequenceMang == null)
                {
                    Debug.LogWarning("SequenceMang konnte nicht gefunden werden.");
                }
            }
        }


        public void ShowWinnerScreen(bool player1Won)
        {
            Time.timeScale = 1f;

            DdCodeEventHandler.Trigger_AtmosphereLevelChanged(AtmoLevel.Atmo_win);
            winnerPanel.SetActive(true);
            buttonsContainer.SetActive(false);

            if (player1Visual == null || player2Visual == null)
            {
                Debug.LogError("Visuals not assigned!");
            }

            if (player1Won)
            {
                Debug.Log("Animating Player 1 as winner");
                StartCoroutine(AnimateVisual(player1Visual, winnerScale, normalColor, true));
                StartCoroutine(AnimateVisual(player2Visual, loserScale, loserColor, false));
            }
            else
            {
                Debug.Log("Animating Player 2 as winner");
                StartCoroutine(AnimateVisual(player2Visual, winnerScale, normalColor, true));
                StartCoroutine(AnimateVisual(player1Visual, loserScale, loserColor, false));
            }

            StartCoroutine(ShowButtonsDelayed());
        }


        private IEnumerator AnimateVisual(SkeletonGraphic visual, float targetScale, Color targetColor, bool win)
        {
            if (win)
            {
                visual.AnimationState.SetAnimation(0, WinnerAnimation, true);
            }

            Vector3 initialScale = visual.rectTransform.localScale;
            Vector3 finalScale = Vector3.one * targetScale;
            Color initialColor = visual.color;
            float elapsed = 0f;

            while (elapsed < scaleDuration)
            {
                Time.timeScale = 1f;
                float t = elapsed / scaleDuration;
                visual.rectTransform.localScale = Vector3.Lerp(initialScale, finalScale, t);
                visual.color = Color.Lerp(initialColor, targetColor, t);

                elapsed += Time.deltaTime;
                yield return null;
            }

            visual.rectTransform.localScale = finalScale;
            visual.color = targetColor;
        }


        private IEnumerator ShowButtonsDelayed()
        {
            yield return new WaitForSeconds(delayBeforeButtons);
            buttonsContainer.SetActive(true);
        }

        public void OnRestartButton()
        {
            if (sequenceMang != null)
            {
                DdCodeEventHandler.Trigger_GameReset();
                sequenceMang.BackToCardPhase();
            }
            else
            {
                Debug.LogError("SequenceMang ist nicht gesetzt – Restart nicht möglich.");
            }
        }

        public void OnMainMenuButton()
        {
            StartCoroutine(LoadMainMenuWithDelay());
        }

        private IEnumerator LoadMainMenuWithDelay()
        {
            DdCodeEventHandler.Trigger_GameReset();
            yield return new WaitForSeconds(2.5f); // Delay zwischen 1–2 Sekunden
            SceneManager.LoadScene(0);
        }
    }
}