using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace dungeonduell
{
    public class DungeonSceneStartTransition : MonoBehaviour
    {
        [Header("Camera References")]
        [SerializeField] private CinemachineVirtualCamera player1Cam;
        [SerializeField] private CinemachineVirtualCamera player2Cam;

        [Header("UI Overlay")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;

        [Header("Settings")]
        [SerializeField] private float startZoom = 80f;
        [SerializeField] private float targetZoom = 8f;
        [SerializeField] private float zoomDuration = 1f;
        [SerializeField] private float fadeDuration = 0.6f;
        [SerializeField] private float fadeDelay = 0.7f;

        private bool transitionStarted = false;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            if (player1Cam != null)
            {
                player1Cam.m_Lens.OrthographicSize = startZoom;
            }
            if (player2Cam != null)
            {
                player2Cam.m_Lens.OrthographicSize = startZoom;
            }

        }

        void Start()
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
                fadeCanvasGroup.DOFade(0f, fadeDuration)
                    .SetDelay(fadeDelay);
            }
        }
    }
}
