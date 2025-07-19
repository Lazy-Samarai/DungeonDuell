using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Cinemachine;
using MoreMountains.TopDownEngine;
using System.Linq;

namespace dungeonduell
{
    public class GymToCardTransition : MonoBehaviour
    {
        [Header("Camera References")] [SerializeField]
        private CinemachineVirtualCamera player1Cam;

        [SerializeField] private CinemachineVirtualCamera player2Cam;

        [Header("UI Overlay")] [SerializeField]
        private CanvasGroup fadeCanvasGroup;

        [Header("Settings")] [SerializeField] private float targetZoom = 8f;
        [SerializeField] private float zoomDuration = 1f;
        [SerializeField] private float fadeDuration = 0.6f;
        [SerializeField] private float fadeDelay = 0.7f;
        [SerializeField] private int targetSceneIndex = 2;

        private bool transitionStarted = false;

        [SerializeField] bool getCamSelf = false;

        private void OnEnable()
        {
            DdCodeEventHandler.SceneTransition += StartTransition;
        }

        private void OnDisable()
        {
            DdCodeEventHandler.SceneTransition -= StartTransition;
        }

        private void Start()
        {
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.DOFade(0f, fadeDuration)
                    .SetDelay(fadeDelay)
                    .OnComplete(() =>
                    {
                        //SceneManager.LoadScene(targetSceneIndex);
                    });
            }
        }

        public void StartTransition()
        {
            if (transitionStarted) return;
            transitionStarted = true;

            // Dynamisch aktive Kameras finden
            var allCams = FindObjectsByType<CinemachineVirtualCamera>(FindObjectsSortMode.None);
            var activeCams = allCams
                .Where(cam => cam.enabled && cam.gameObject.activeInHierarchy)
                .ToList();

            // Spieler 1 Kamera suchen (z.B. nach Namensmuster)
            player1Cam = activeCams.FirstOrDefault(c => c.name.Contains("P1"));
            player2Cam = activeCams.FirstOrDefault(c => c.name.Contains("P2"));

            Debug.Log($"Found Player1Cam: {player1Cam?.name}, Player2Cam: {player2Cam?.name}");

            // Zoom Kameras (wenn gefunden)
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

            // Fading und Szenenwechsel
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.DOFade(1f, fadeDuration)
                    .SetDelay(fadeDelay)
                    .OnComplete(() => SceneManager.LoadScene(targetSceneIndex));
            }
            else
            {
                SceneManager.LoadScene(targetSceneIndex);
            }
        }

    }
}