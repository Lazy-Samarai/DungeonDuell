using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace dungeonduell
{
    public class LegendSlider : MonoBehaviour
    {
        public RectTransform legendPanel;
        //public InputActionReference toggleLegendAction; // Action für den Input
        public float slideDuration = 0.5f;
        public TextMeshProUGUI legendButton;

        private Vector2 hiddenPosition;
        private Vector2 visiblePosition;
        private bool isVisible = false;

        void Start()
        {
            // Positionen setzen
            hiddenPosition = new Vector2(-legendPanel.rect.width, legendPanel.anchoredPosition.y);
            visiblePosition = new Vector2(164, legendPanel.anchoredPosition.y);

            // Startposition außerhalb des Bildschirms
            legendPanel.anchoredPosition = hiddenPosition;

            // Eventlistener für das Input System registrieren
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
