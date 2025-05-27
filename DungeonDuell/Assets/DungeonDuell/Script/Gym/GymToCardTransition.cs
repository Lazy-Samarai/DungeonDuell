using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Cinemachine;

namespace dungeonduell
{
    public class GymToCardTransition : MonoBehaviour
    {
        [Header("Camera References")]
        [SerializeField] private CinemachineVirtualCamera player1Cam;
        [SerializeField] private CinemachineVirtualCamera player2Cam;

        [Header("UI Overlay")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;

        [Header("Settings")]
        [SerializeField] private float targetZoom = 8f;
        [SerializeField] private float zoomDuration = 1f;
        [SerializeField] private float fadeDuration = 0.6f;
        [SerializeField] private float fadeDelay = 0.7f;
        [SerializeField] private int targetSceneIndex = 2;

        private bool transitionStarted = false;

        private void OnEnable()
        {
            DdCodeEventHandler.GymPhaseFinished += StartTransition;
        }

        private void OnDisable()
        {
            DdCodeEventHandler.GymPhaseFinished -= StartTransition;
        }

        public void StartTransition()
        {
            if (transitionStarted) return;
            transitionStarted = true;

            // Zoom beide Kameras
            if (player1Cam != null)
            {
                DOTween.To(() => player1Cam.m_Lens.OrthographicSize,
                           x => player1Cam.m_Lens.OrthographicSize = x,
                           targetZoom,
                           zoomDuration).SetEase(Ease.InOutCubic);
            }

            if (player2Cam != null)
            {
                DOTween.To(() => player2Cam.m_Lens.OrthographicSize,
                           x => player2Cam.m_Lens.OrthographicSize = x,
                           targetZoom,
                           zoomDuration).SetEase(Ease.InOutCubic);
            }

            // Fading
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.DOFade(1f, fadeDuration)
                    .SetDelay(fadeDelay)
                    .OnComplete(() =>
                    {
                        SceneManager.LoadScene(targetSceneIndex);
                    });
            }
            else
            {
                SceneManager.LoadScene(targetSceneIndex);
            }
        }
    }
}
