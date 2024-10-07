using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

namespace dungeonduell
{
    public class DisplayCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Card card;
        
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI roomTypeText;
        public Image cardImage;
        public DoorIndicator cardDirectionIndiactor;

        private Transform cardTransform;
        private Vector3 originalScale;
        private Quaternion originalRotation; // Speichert die ursprüngliche Rotation der Karte
        private Vector3 handPanelOriginalPosition; // Speichert die Position der Karte im HandPanel in Weltkoordinaten
        private int originalSiblingIndex; // Speichert den ursprünglichen Sibling Index der Karte im HandPanel

        public GameObject tooltip;
        public GameObject cardHolder; // Referenz zum CardHolder GameObject
        public Transform handPanel;  // Referenz zum HandPanel

        public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f); // Skalierung der Karte bei Hover
        public Vector3 hoverOffset = new Vector3(0f, 20f, 0f); // Verschiebung der Karte bei Hover (Y-Achse)
        public Vector3 sideOffset = new Vector3(30f, 0f, 0f); // Verschiebung der Nachbarkarten seitlich

        void Start()
        {
            cardTransform = GetComponent<Transform>();
            originalScale = cardTransform.localScale;
            originalRotation = cardTransform.localRotation; // Die ursprüngliche Rotation speichern

            HideTooltip();
            UpdateCardDisplay();

            
            // Versuche, das CardHolder- und HandPanel-GameObject zu finden
            if (cardHolder == null)
            {
                cardHolder = GameObject.Find("CardHolder");
                if (cardHolder == null)
                {
                    Debug.LogError("CardHolder GameObject nicht gefunden.");
                }
            }
            

            if (handPanel == null)
            {
                handPanel = GameObject.Find("HandPanel")?.transform;
                if (handPanel == null)
                {
                    Debug.LogError("HandPanel GameObject nicht gefunden.");
                }
            }

            // Speichere die Position im HandPanel und den ursprünglichen Sibling Index, wenn die Karte Teil des HandPanels ist
            if (cardTransform.parent == handPanel)
            {
                handPanelOriginalPosition = cardTransform.position; // Speichert die Position in Weltkoordinaten
                originalSiblingIndex = transform.GetSiblingIndex(); // Speichert den ursprünglichen Sibling Index
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (cardTransform != null && cardHolder != null)
            {
                if (cardTransform.parent != cardHolder.transform) // Hover-Effekt nur anwenden, wenn die Karte nicht auf dem CardHolder ist
                {
                    cardTransform.localScale = hoverScale; // Skalierung der Karte bei Hover
                    cardTransform.localPosition += hoverOffset; // Verschiebe die Karte nach oben (Y-Achse)
                    cardTransform.localRotation = Quaternion.Euler(0f, 0f, 0f); // Setzt die Rotation temporär auf 0

                    // Tooltip anzeigen
                    if (tooltip != null)
                    {
                        tooltip.SetActive(true);

                        // Aktualisiere den Tooltip-Text mit der Beschreibung der Karte
                        TextMeshProUGUI tooltipText = tooltip.GetComponentInChildren<TextMeshProUGUI>();
                        if (tooltipText != null)
                        {
                            tooltipText.text = card.cardDescription;
                        }
                    }

                    // Bewege die linke und rechte Nachbarkarte
                    MoveAdjacentCards(true);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (cardTransform != null && cardHolder != null)
            {
                if (cardTransform.parent != cardHolder.transform) // Hover-Effekt nur anwenden, wenn die Karte nicht auf dem CardHolder ist
                {
                    // Setze die Skalierung, Position und Rotation der Karte auf die ursprünglichen Werte zurück
                    cardTransform.localScale = originalScale;
                    cardTransform.localPosition -= hoverOffset; // Setzt die Y-Position zurück
                    cardTransform.localRotation = originalRotation;

                    // Tooltip ausblenden
                    if (tooltip != null)
                    {
                        tooltip.SetActive(false);
                    }

                    // Setze die linke und rechte Nachbarkarte zurück
                    MoveAdjacentCards(false);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            HandleCardClick(); // Verarbeite den Klick auf die Karte
        }

        private void HandleCardClick()
        {
            Debug.Log($"Karte geklickt: {card.cardName}");

            if (cardTransform.parent == cardHolder.transform)
            {
                // Wenn die Karte bereits auf dem CardHolder ist, bewege sie zurück ins HandPanel
                cardTransform.SetParent(handPanel); // Setze den Parent auf das HandPanel
                cardTransform.position = new Vector3(handPanelOriginalPosition.x, handPanelOriginalPosition.y + 250f, handPanelOriginalPosition.z); // Bewege die Karte an ihre ursprüngliche Position
                cardTransform.localScale = originalScale; // Setze die ursprüngliche Skalierung zurück
                cardTransform.localRotation = originalRotation; // Setze die ursprüngliche Rotation zurück
                transform.SetSiblingIndex(originalSiblingIndex); // Setze den ursprünglichen Sibling Index zurück

                // Die Karte wurde vom CardHolder entfernt, daher currentCard auf null setzen
                TileClickHandler tileClickHandler = FindObjectOfType<TileClickHandler>();
                if (tileClickHandler != null)
                {
                    tileClickHandler.ChangeCard(null,null,null);
                }
            }
            else
            {
                // Überprüfe, ob bereits eine Karte auf dem CardHolder ist
                if (cardHolder.transform.childCount > 0)
                {
                    Transform cardOnHolder = cardHolder.transform.GetChild(0); // Die Karte, die sich aktuell auf dem CardHolder befindet

                    // Bewege die Karte vom CardHolder zurück zur Hand
                    DisplayCard cardOnHolderScript = cardOnHolder.GetComponent<DisplayCard>();
                    if (cardOnHolderScript != null)
                    {
                        cardOnHolderScript.UpdateDirectionIndicator(card.GetAllowedDirection());
                        cardOnHolderScript.cardTransform.SetParent(handPanel);
                        cardOnHolderScript.cardTransform.position = cardOnHolderScript.handPanelOriginalPosition; // Setze die Karte an ihre ursprüngliche Position zurück
                        cardOnHolderScript.cardTransform.localScale = cardOnHolderScript.originalScale;
                        cardOnHolderScript.cardTransform.localRotation = cardOnHolderScript.originalRotation;
                        cardOnHolderScript.transform.SetSiblingIndex(cardOnHolderScript.originalSiblingIndex); // Setze den ursprünglichen Sibling Index zurück
                        tooltip.SetActive(false); // Blende den Tooltip aus
                    }
                }

                // Bewege die aktuell geklickte Karte auf den CardHolder
                cardTransform.SetParent(cardHolder.transform); // Setze den Parent auf den CardHolder
                cardTransform.position = cardHolder.transform.position; // Setze die Position auf die des CardHolders
                cardTransform.localScale = Vector3.one; // Setze die Skalierung auf 1
                cardTransform.localRotation = Quaternion.identity; // Setze die Rotation auf 0

                // Die Karte auf dem CardHolder ist jetzt die aktuelle Karte
                TileClickHandler tileClickHandler = FindObjectOfType<TileClickHandler>();
                if (tileClickHandler != null)
                {
                  
                    tileClickHandler.ChangeCard(card, card.GetAllowedDirection(),this); // working in Ui still has to be done 
                }
            }
        }

        private void MoveAdjacentCards(bool isHovering)
        {
            int siblingIndex = transform.GetSiblingIndex();

            // Verschiebe die linke Nachbarkarte
            if (siblingIndex > 0)
            {
                Transform leftCard = handPanel.GetChild(siblingIndex - 1);
                if (isHovering)
                {
                    leftCard.localPosition -= sideOffset; // Bewege nach links
                }
                else
                {
                    leftCard.localPosition += sideOffset; // Bewege zurück
                }
            }

            // Verschiebe die rechte Nachbarkarte
            if (siblingIndex < handPanel.childCount - 1)
            {
                Transform rightCard = handPanel.GetChild(siblingIndex + 1);
                if (isHovering)
                {
                    rightCard.localPosition += sideOffset; // Bewege nach rechts
                }
                else
                {
                    rightCard.localPosition -= sideOffset; // Bewege zurück
                }
            }
        }

        public void UpdateCardDisplay()
        {
            if (card != null)
            {
                // Aktualisiere die Text- und Bildkomponenten basierend auf den Daten der Karte
                nameText.text = card.cardName;
                descriptionText.text = card.cardDescription;
                roomTypeText.text = "Room Type: " + card.roomtype.ToString();
                UpdateDirectionIndicator(card.GetAllowedDirection());


                if (card.cardImage != null)
                {
                    cardImage.sprite = card.cardImage;
                }
            }
        }

        private void HideTooltip()
        {
            if (tooltip != null)
            {
                tooltip.SetActive(false); // Blendet den Tooltip aus
            }
        }
        public void UpdateDirectionIndicator(bool[] allowedDoors)
        {
            cardDirectionIndiactor.SetDoorIndiactor(allowedDoors);
        }
    }
}
