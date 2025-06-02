using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.VisualScripting;

namespace dungeonduell
{
    public class SceneLoading : MonoBehaviour
    {

        [Header("Camera References")]
        [SerializeField] private CinemachineVirtualCamera player1Cam;
        [SerializeField] private CinemachineVirtualCamera player2Cam;

        [Header("UI Overlay")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;

        [Header("Settings")]
        [SerializeField] private float targetZoom = 0f;
        [SerializeField] private float zoomDuration = 1f;
        [SerializeField] private float fadeDuration = 0.6f;
        [SerializeField] private float fadeDelay = 0.7f;
        [SerializeField] private int targetSceneIndex = 2;

        private bool transitionStarted = false;

        public void ToTheDungeon()
        {
            SceneManager.LoadScene(2);
        }

        public void ToTheHex()
        {
            SceneManager.LoadScene(1);
        }

        public void ToTheDungeonTransition()
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
        }


        private void OnEnable()
        {
            DdCodeEventHandler.SceneTransition += ToTheDungeonTransition;
        }

        private void OnDisable()
        {
            DdCodeEventHandler.SceneTransition -= ToTheDungeonTransition;
        }
    }
}