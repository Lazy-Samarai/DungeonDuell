using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace dungeonduell
{
    public class CardDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Card card;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI roomTypeText;
        public Image cardImage;

        private Transform cardTransform;
        private Vector3 originalScale;
        private Quaternion originalRotation;

        // Tooltip UI-Komponenten
        public GameObject tooltip;

        public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f);
        public Vector3 hoverOffset = new Vector3(0f, 20f, 0f);

        void Start()
        {
            // Transform der Karte selbstständig holen
            cardTransform = GetComponent<Transform>();
            originalScale = cardTransform.localScale;
            originalRotation = cardTransform.localRotation;

            HideTooltip();
            UpdateCardDisplay();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            cardTransform.localScale = hoverScale;
            cardTransform.localPosition += hoverOffset;
            cardTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            // Tooltip anzeigen
            if (tooltip != null)
            {
                tooltip.SetActive(true);
               
                // Tooltip-Text aktualisieren
                TextMeshProUGUI tooltipText = tooltip.GetComponentInChildren<TextMeshProUGUI>();
                if (tooltipText != null)
                {
                    tooltipText.text = card.cardDescription;
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Zurück zur ursprünglichen Größe und Rotation
            cardTransform.localScale = originalScale;
            cardTransform.localPosition -= hoverOffset;
            cardTransform.localRotation = originalRotation;

            // Tooltip ausblenden
            if (tooltip != null)
            {
                tooltip.SetActive(false);
            }
        }

        // Methode zum Aktualisieren der Kartendetails im UI
        public void UpdateCardDisplay()
        {
            if (card != null)
            {
                nameText.text = card.cardName;
                descriptionText.text = card.cardDescription;
                roomTypeText.text = "Room Type: " + card.roomtype.ToString();

                if (card.cardImage != null)
                {
                    cardImage.sprite = card.cardImage.sprite;
                }
            }
        }

        // Tooltip verstecken
        private void HideTooltip()
        {
            if (tooltip != null)
            {
                tooltip.SetActive(false);
            }
        }
    }
}
