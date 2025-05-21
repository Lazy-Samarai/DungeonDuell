using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace dungeonduell
{
    public class SceneCurtain : MonoBehaviour
    {
        public RectTransform topPanel;
        public RectTransform bottomPanel;
        public float animationDuration = 0.5f;

        public GameObject tutorialSlideshow;

        private void Start()
        {
            /* Vorhänge öffnen beim Start
            topPanel.anchoredPosition = new Vector2(0, 0);
            bottomPanel.anchoredPosition = new Vector2(0, 0);

            topPanel.DOAnchorPosY(540, animationDuration).SetEase(Ease.InOutSine);
            bottomPanel.DOAnchorPosY(-540, animationDuration).SetEase(Ease.InOutSine);*/
        }

        public void StartTransitionToScene()
        {
            topPanel.DOAnchorPosY(-540, animationDuration).SetEase(Ease.InOutSine);
            bottomPanel.DOAnchorPosY(540, animationDuration).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                if (OptionDataManager.Instance.showTutorial)
                {
                    tutorialSlideshow.SetActive(true);
                    return;
                }
            });
        }
    }
}
