using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

namespace dungeonduell
{
    public class CreditsScroller : MonoBehaviour
    {
        public RectTransform creditsText;
        public float scrollSpeed = 50f;
        public float resetPosition = -600f;
        public float startPosition = 600f;
        public GameObject creditsPanel;
        private Tweener scrollTween;

        void OnEnable()
        {
            StartScrolling();
        }

        void StartScrolling()
        {
            if (creditsText == null) return;

            creditsText.anchoredPosition = new Vector2(creditsText.anchoredPosition.x, resetPosition);
            scrollTween = creditsText.DOAnchorPosY(startPosition, scrollSpeed, false)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }
    }
}
