using DG.Tweening;
using UnityEngine;

namespace dungeonduell
{
    public class CreditsScroller : MonoBehaviour
    {
        public RectTransform creditsText;
        public float scrollSpeed = 50f;
        public float resetPosition = -600f;
        public float startPosition = 600f;
        public GameObject creditsPanel;
        private Tweener _scrollTween;

        private void OnEnable()
        {
            StartScrolling();
        }

        private void StartScrolling()
        {
            if (creditsText == null) return;

            creditsText.anchoredPosition = new Vector2(creditsText.anchoredPosition.x, resetPosition);
            _scrollTween = creditsText.DOAnchorPosY(startPosition, scrollSpeed)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }
    }
}