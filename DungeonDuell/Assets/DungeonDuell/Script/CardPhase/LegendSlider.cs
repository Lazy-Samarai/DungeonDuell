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

        private Vector2 _hiddenPosition;
        private bool _isVisible;
        private Vector2 _visiblePosition;

        private void Start()
        {
            // Positionen setzen
            _hiddenPosition = new Vector2(-legendPanel.rect.width, legendPanel.anchoredPosition.y);
            _visiblePosition = new Vector2(164, legendPanel.anchoredPosition.y);

            // Startposition au�erhalb des Bildschirms
            legendPanel.anchoredPosition = _hiddenPosition;

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
            _isVisible = !_isVisible;
            legendButton.text = (_isVisible ? "<-" : "->") + " Legend";
            legendPanel.DOAnchorPos(_isVisible ? _visiblePosition : _hiddenPosition, slideDuration)
                .SetEase(Ease.OutCubic);
        }
    }
}