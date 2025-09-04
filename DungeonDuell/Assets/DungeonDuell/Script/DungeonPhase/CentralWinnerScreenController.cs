using System.Collections;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

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
        private VCA _vcaSfxNoUi;
        private VCA _vcaSfx;

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

            _vcaSfxNoUi = RuntimeManager.GetVCA("vca:/SFX_NO_UI");
            _vcaSfx = RuntimeManager.GetVCA("vca:/SFX");
        }


        public void ShowWinnerScreen(bool player1Won)
        {
            Time.timeScale = 1f;

            DdCodeEventHandler.Trigger_AtmosphereLevelChanged(AtmoLevel.Atmo_win);
            winnerPanel.SetActive(true);
            buttonsContainer.SetActive(false);

            if (_vcaSfxNoUi.isValid())
                _vcaSfxNoUi.setVolume(0f);

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

        private void RestoreSfxNoUiToCurrentSfx()
        {
            float target = 1f;
            if (_vcaSfx.isValid())
                _vcaSfx.getVolume(out target);      // aktuellen SFX-Level holen (Slider kann sich geändert haben)
            if (_vcaSfxNoUi.isValid())
                _vcaSfxNoUi.setVolume(target);      // SFX_NO_UI darauf setzen
        }

        public void OnRestartButton()
        {
            RestoreSfxNoUiToCurrentSfx();

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
            RestoreSfxNoUiToCurrentSfx();
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