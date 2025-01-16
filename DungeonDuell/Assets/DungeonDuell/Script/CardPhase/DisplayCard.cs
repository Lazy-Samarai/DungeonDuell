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
        public Image HexImage;
        public DoorIndicator cardDirectionIndiactor;

        public GameObject MonsterRoomIcon;
        public GameObject TreasureRoomIcon;
        public GameObject Frame;

        private Transform cardTransform;
        private Vector3 originalScale;
        private Quaternion originalRotation;
        private Vector3 handPanelOriginalPosition;
        private int originalSiblingIndex;

        public GameObject tooltip;
        public GameObject cardHolder;
        public Transform handPanel;

        public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f); // Größe der Karten beim Hover
        public Vector3 hoverOffset = new Vector3(0f, 20f, 0f);
        public Vector3 sideOffset = new Vector3(30f, 0f, 0f);
        public float fanAngle = 10f; // Winkel für das Auffächern der Karten

        void Start()
        {
            cardTransform = GetComponent<Transform>();
            originalScale = cardTransform.localScale;
            originalRotation = cardTransform.localRotation;

            HideTooltip();
            UpdateCardDisplay();

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

            if (cardTransform.parent == handPanel)
            {
                handPanelOriginalPosition = cardTransform.position;
                originalSiblingIndex = transform.GetSiblingIndex();
            }
        }

        public void UpdateCardDisplay()
        {
            if (card != null)
            {
                nameText.text = card.cardName;

                if (HexImage != null)
                {
                    switch (card.roomtype)
                    {
                        case RoomType.Enemy:
                            HexImage.color = Color.red;
                            Frame.GetComponent<Image>().color = Color.red;
                            MonsterRoomIcon.SetActive(true);
                            TreasureRoomIcon.SetActive(false);
                            MonsterRoomIcon.GetComponent<Image>().color = Color.red;
                            nameText.color = Color.red;
                            break;
                        case RoomType.NormalLott:
                            HexImage.color = Color.yellow;
                            Frame.GetComponent<Image>().color = Color.yellow;
                            TreasureRoomIcon.SetActive(true);
                            MonsterRoomIcon.SetActive(false);
                            TreasureRoomIcon.GetComponent<Image>().color = Color.yellow;
                            nameText.color = Color.yellow;
                            break;
                        default:
                            HexImage.color = new Color(0.3f, 0.3f, 0.3f);
                            Frame.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
                            MonsterRoomIcon.SetActive(false);
                            TreasureRoomIcon.SetActive(false);
                            nameText.color = new Color(0.3f, 0.3f, 0.3f);
                            break;
                    }
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (cardTransform != null && cardHolder != null)
            {
                if (cardTransform.parent != cardHolder.transform)
                {
                    cardTransform.localScale = hoverScale;
                    cardTransform.localPosition += hoverOffset;
                    cardTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                    if (tooltip != null)
                    {
                        tooltip.SetActive(true);

                        TextMeshProUGUI tooltipText = tooltip.GetComponentInChildren<TextMeshProUGUI>();
                        if (tooltipText != null)
                        {
                            tooltipText.text = card.cardDescription;
                        }
                    }

                    MoveAllCards(true);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (cardTransform != null && cardHolder != null)
            {
                if (cardTransform.parent != cardHolder.transform)
                {
                    cardTransform.localScale = originalScale;
                    cardTransform.localPosition -= hoverOffset;
                    cardTransform.localRotation = originalRotation;

                    if (tooltip != null)
                    {
                        tooltip.SetActive(false);
                    }

                    MoveAllCards(false);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            HandleCardClick();
        }

        private void HandleCardClick()
        {
            Debug.Log($"Karte geklickt: {card.cardName}");

            if (cardTransform.parent == cardHolder.transform)
            {
                cardTransform.SetParent(handPanel);
                cardTransform.position = new Vector3(handPanelOriginalPosition.x, handPanelOriginalPosition.y + 250f, handPanelOriginalPosition.z);
                cardTransform.localScale = originalScale;
                cardTransform.localRotation = originalRotation;
                transform.SetSiblingIndex(originalSiblingIndex);

                TileClickHandler tileClickHandler = FindObjectOfType<TileClickHandler>();
                if (tileClickHandler != null)
                {
                    tileClickHandler.ChangeCard(null, null, null);
                }
            }
            else
            {
                if (cardHolder.transform.childCount > 0)
                {
                    Transform cardOnHolder = cardHolder.transform.GetChild(0);

                    DisplayCard cardOnHolderScript = cardOnHolder.GetComponent<DisplayCard>();
                    if (cardOnHolderScript != null)
                    {
                        cardOnHolderScript.UpdateDirectionIndicator(card.GetAllowedDirection());
                        cardOnHolderScript.cardTransform.SetParent(handPanel);
                        cardOnHolderScript.cardTransform.position = cardOnHolderScript.handPanelOriginalPosition;
                        cardOnHolderScript.cardTransform.localScale = cardOnHolderScript.originalScale;
                        cardOnHolderScript.cardTransform.localRotation = cardOnHolderScript.originalRotation;
                        cardOnHolderScript.transform.SetSiblingIndex(cardOnHolderScript.originalSiblingIndex);
                        cardOnHolderScript.HideTooltip(); // Tooltip des alten CardHolder-Karte ausblenden
                    }
                }

                cardTransform.SetParent(cardHolder.transform);
                cardTransform.position = cardHolder.transform.position;
                cardTransform.localScale = Vector3.one;
                cardTransform.localRotation = Quaternion.identity;

                HideTooltip(); // Tooltip für aktuelle Karte ausblenden

                TileClickHandler tileClickHandler = FindObjectOfType<TileClickHandler>();
                if (tileClickHandler != null)
                {
                    tileClickHandler.ChangeCard(card, card.GetAllowedDirection(), this);
                }
            }
        }

        private void MoveAllCards(bool isHovering)
        {
            for (int i = 0; i < handPanel.childCount; i++)
            {
                Transform card = handPanel.GetChild(i);
                if (card == transform) continue;

                if (isHovering)
                {
                    card.localPosition += sideOffset * (i < transform.GetSiblingIndex() ? -1 : 1);
                }
                else
                {
                    card.localPosition -= sideOffset * (i < transform.GetSiblingIndex() ? -1 : 1);
                }
            }
        }

        private void HideTooltip()
        {
            if (tooltip != null)
            {
                tooltip.SetActive(false);
            }
        }

        public void UpdateDirectionIndicator(bool[] allowedDoors)
        {
            cardDirectionIndiactor.SetDoorIndiactor(allowedDoors);
        }
    }
}
