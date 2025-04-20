using DG.Tweening;
using TMPro;
using UnityEngine;

namespace dungeonduell
{
    public class LegendSlider : MonoBehaviour
    {
        public RectTransform legendPanel;

        //public InputActionReference toggleLegendAction; // Action f�r den Input
        public float slideDuration = 0.5f;
        public TextMeshProUGUI legendButton;

        private Vector2 hiddenPosition;
        private bool isVisible;
        private Vector2 visiblePosition;

        private void Start()
        {
            // Positionen setzen
            hiddenPosition = new Vector2(-legendPanel.rect.width, legendPanel.anchoredPosition.y);
            visiblePosition = new Vector2(164, legendPanel.anchoredPosition.y);

            // Startposition au�erhalb des Bildschirms
            legendPanel.anchoredPosition = hiddenPosition;

            // Eventlistener f�r das Input System registrieren
            //toggleLegendAction.action.performed += ctx => ToggleLegend();
        }

        private void OnDestroy()
        {
            // Eventlistener entfernen, um Memory Leaks zu vermeiden
            //toggleLegendAction.action.performed -= ctx => ToggleLegend();
        }

        public void ToggleLegend()
        {
            isVisible = !isVisible;
            legendButton.text = (isVisible ? "<-" : "->") + " Legend";
            legendPanel.DOAnchorPos(isVisible ? visiblePosition : hiddenPosition, slideDuration).SetEase(Ease.OutCubic);
        }
    }
}