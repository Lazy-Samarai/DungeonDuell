using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Cinemachine;
using MoreMountains.TopDownEngine;

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

            if (getCamSelf)
            {
                CinemachineCameraController[] cams =
                    FindObjectsByType<CinemachineCameraController>(FindObjectsSortMode.None);

                player1Cam = cams[0].GetComponent<CinemachineVirtualCamera>();
                player2Cam = cams[1].GetComponent<CinemachineVirtualCamera>();
                Animator camAni1 = player1Cam.GetComponent<Animator>();
                Animator camAni2 = player1Cam.GetComponent<Animator>();
                if (camAni1 != null) camAni1.enabled = false;
                if (camAni2 != null) camAni2.enabled = false;
            }

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
                    .OnComplete(() => { SceneManager.LoadScene(targetSceneIndex); });
            }
            else
            {
                SceneManager.LoadScene(targetSceneIndex);
            }
        }
    }
}