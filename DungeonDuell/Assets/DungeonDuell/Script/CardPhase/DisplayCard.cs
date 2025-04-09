using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace dungeonduell
{
    public class DisplayCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
    {
        public Card card;
        public CardToHand cardToHand;
        public Transform handPanel;

        [Header("UI References")]
        public TextMeshProUGUI nameText;
        public Image HexImage;
        public DoorIndicator cardDirectionIndicator;
        public GameObject MonsterRoomIcon;
        public GameObject TreasureRoomIcon;
        public GameObject normalBG;
        public GameObject enemyBG;
        public GameObject lootBG;

        public GameObject Frame;
        public GameObject tooltip;

        [Header("Hover-Effekt")]
        public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f);
        public Vector3 hoverOffset = new Vector3(0f, 20f, 0f);
        public Vector3 sideOffset = new Vector3(30f, 0f, 0f);

        private Vector3 originalScale;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        
//[SerializeField] private Sprite[] spritesFullCard;

        void Start()
        {
            if (handPanel == null)
            {
                CardToHand cth = GetComponentInParent<CardToHand>();
                if (cth != null)
                {
                    handPanel = cth.handPanel;
                }
                else
                {
                    Debug.LogError("Kein CardToHand im Parent gefunden. HandPanel kann nicht ermittelt werden!");
                }
            }
            originalScale = transform.localScale;
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;

            HideTooltip();
            UpdateCardDisplay();

            
        }

        public void UpdateCardDisplay()
        {
            if (card != null)
            {
                if (nameText != null)
                    nameText.text = card.cardName;

                if (cardDirectionIndicator != null)
                {
                    bool[] allowedDoors = card.GetAllowedDirection();
                    cardDirectionIndicator.SetDoorIndiactor(allowedDoors);
                }

                if (HexImage != null && Frame != null)
                {
                    Color currentColor = new Color(0.3f, 0.3f, 0.3f);

                    switch (card.roomtype)
                    {
                        case RoomType.Enemy:
                            MonsterRoomIcon?.SetActive(true);
                            TreasureRoomIcon?.SetActive(false);
                            enemyBG?.SetActive(true);
                            lootBG?.SetActive(false);
                            normalBG?.SetActive(false);
                            break;

                        case RoomType.NormalLott:
                            MonsterRoomIcon?.SetActive(false);
                            TreasureRoomIcon?.SetActive(true);
                            enemyBG?.SetActive(false);
                            lootBG?.SetActive(true);
                            normalBG?.SetActive(false);
                            break;

                        default: // Generic oder alles andere
                            MonsterRoomIcon?.SetActive(false);
                            TreasureRoomIcon?.SetActive(false);
                            enemyBG?.SetActive(false);
                            lootBG?.SetActive(false);
                            normalBG?.SetActive(true);
                            break;
                    }


                    if (Frame.GetComponent<Image>() != null)
                        Frame.GetComponent<Image>().color = currentColor;
                    HexImage.color = currentColor;

                    if (nameText != null)
                        nameText.color = currentColor;
                    MonsterRoomIcon.GetComponent<Image>().color = currentColor;
                    TreasureRoomIcon.GetComponent<Image>().color = currentColor;
                }
            }
        }

        public void UpdateDirectionIndicator(bool[] allowedDoors)
        {
            if (cardDirectionIndicator != null)
                cardDirectionIndicator.SetDoorIndiactor(allowedDoors);
        }

        // **Hover mit Maus**
        public void OnPointerEnter(PointerEventData eventData)
        {
            ActivateHoverEffect();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DeactivateHoverEffect();
        }

        // **Hover aktivieren, wenn Karte mit Controller ausgewählt wird**
        public void OnSelect(BaseEventData eventData)
        {
            ActivateHoverEffect();
        }

        // **NEU: Hover deaktivieren, wenn Karte mit Controller abgewählt wird**
        public void OnDeselect(BaseEventData eventData)
        {
            DeactivateHoverEffect();
        }

        // **Klick-Verhalten**
        public void OnPointerClick(PointerEventData eventData)
        {
            DeactivateHoverEffect();
            if (cardToHand != null)
            {
                cardToHand.OnCardClicked(this);
            }
            else
            {
                Debug.LogWarning("Keine cardToHand-Referenz in DisplayCard vorhanden!");
            }
        }

        public void ActivateHoverEffect()
        {
            if (transform.parent == handPanel)
            {
                transform.localScale = hoverScale;
                transform.localRotation = Quaternion.identity;
                transform.localPosition += hoverOffset;

                if (tooltip != null)
                {
                    var tmp = tooltip.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmp != null && card != null)
                    {
                        tmp.text = card.cardDescription;
                    }

                    AdjustNeighborCards(true);
                }
            }
        }

        private void DeactivateHoverEffect()
        {
            if (transform.parent == handPanel)
            {
                transform.localScale = originalScale;
                transform.localRotation = originalRotation;
                transform.localPosition = originalPosition;

                HideTooltip();
                AdjustNeighborCards(false);
            }
        }

        private void AdjustNeighborCards(bool isHovering)
        {
            Transform parent = transform.parent;
            if (parent == null) return;

            int currentIndex = transform.GetSiblingIndex();

            if (currentIndex > 0)
            {
                Transform leftNeighbor = parent.GetChild(currentIndex - 1);
                if (leftNeighbor.TryGetComponent<DisplayCard>(out var leftCard))
                {
                    Vector3 offset = isHovering ? -sideOffset : sideOffset;
                    leftNeighbor.localPosition += offset;
                }
            }

            if (currentIndex < parent.childCount - 1)
            {
                Transform rightNeighbor = parent.GetChild(currentIndex + 1);
                if (rightNeighbor.TryGetComponent<DisplayCard>(out var rightCard))
                {
                    Vector3 offset = isHovering ? sideOffset : -sideOffset;
                    rightNeighbor.localPosition += offset;
                }
            }
        }

        private void HideTooltip()
        {
            if (tooltip != null)
                tooltip.SetActive(false);
        }
    }
}
