using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace dungeonduell
{
    public class SceneCurtain : MonoBehaviour
    {
        public RectTransform topPanel;
        private float topPanelPosY = 540f;
        public RectTransform bottomPanel;
        private float bottomPanelPosY = -540f;
        public float animationDuration = 1f;
        public bool openGym;

        public GameObject tutorialSlideshow;

        [Tooltip("Index aus Build Settings")] public int targetSceneIndex = -1;

        private void Start()
        {
            if (openGym)
            {
                OpenCurtain();
            }
        }

        public void StartTransitionToScene()
        {
            topPanel.DOAnchorPosY(bottomPanelPosY, animationDuration).SetEase(Ease.InOutSine);
            bottomPanel.DOAnchorPosY(topPanelPosY, animationDuration).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                if (FindFirstObjectByType<OptionDataManager>().showTutorial)
                {
                    tutorialSlideshow.SetActive(true);
                    return;
                }
                else
                {
                    SceneManager.LoadScene(targetSceneIndex);
                }
            });
        }


        public void OpenCurtain()
        {
            topPanel.DOAnchorPosY(topPanelPosY, animationDuration).SetEase(Ease.InOutSine);
            bottomPanel.DOAnchorPosY(bottomPanelPosY, animationDuration).SetEase(Ease.InOutSine);
        }
    }
}