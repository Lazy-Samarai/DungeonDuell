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

        protected virtual void Start()
        {
            if (openGym)
            {
                OpenCurtain();
            }
        }

        public void StartTransitionToScene()
        {
            topPanel.DOAnchorPosY(bottomPanelPosY, animationDuration).SetUpdate(true).SetEase(Ease.InOutSine);
            bottomPanel.DOAnchorPosY(topPanelPosY, animationDuration).SetUpdate(true).SetEase(Ease.InOutSine)
                .OnComplete(() =>
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


        protected void OpenCurtain()
        {
            ChangeCurtainSingle(true, true);
            ChangeCurtainSingle(false, true);
        }

        protected void ChangeCurtainSingle(bool player1, bool open)
        {
            if (player1)
            {
                topPanel.DOAnchorPosY(open ? topPanelPosY : bottomPanelPosY, animationDuration).SetUpdate(true)
                    .SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        if (!open)
                        {
                            tutorialSlideshow.SetActive(true);
                        }
                    });
            }
            else
            {
                bottomPanel.DOAnchorPosY(open ? bottomPanelPosY : topPanelPosY, animationDuration).SetUpdate(true)
                    .SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        if (!open)
                        {
                            tutorialSlideshow.SetActive(true);
                        }
                    });
                ;
            }
        }

        public void CloseCurtain()
        {
            ChangeCurtainSingle(true, false);
            ChangeCurtainSingle(false, false);
        }

        private void OnEnable()
        {
            DdCodeEventHandler.TutorialDone += OpenCurtain;
        }

        private void OnDisable()
        {
            DdCodeEventHandler.TutorialDone -= OpenCurtain;
        }
    }
}