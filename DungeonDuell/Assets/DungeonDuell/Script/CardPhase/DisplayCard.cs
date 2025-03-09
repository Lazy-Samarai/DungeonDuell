using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace dungeonduell
{
    public class DisplayCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Card card;
        // Referenz auf das CardToHand, zu dem diese Karte gehört
        public CardToHand cardToHand;
        public Transform handPanel;     

        [Header("UI References")]
        public TextMeshProUGUI nameText;
        public Image HexImage;
        public DoorIndicator cardDirectionIndicator;
        public GameObject MonsterRoomIcon;
        public GameObject TreasureRoomIcon;
        public GameObject Frame;
        public GameObject tooltip;

        [Header("Hover-Effekt")]
        public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f);
        public Vector3 hoverOffset = new Vector3(0f, 20f, 0f);
        public Vector3 sideOffset = new Vector3(30f, 0f, 0f);

        private Vector3 originalScale;
        private Vector3 originalPosition;
        private Quaternion originalRotation;

        void Start()
        {
            
            if (handPanel == null)
            {
                // Suche in den übergeordneten Objekten nach einem CardToHand
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
                // Name
                if (nameText != null)
                    nameText.text = card.cardName;

                // Allowed Doors / Direction
                if (cardDirectionIndicator != null)
                {
                    bool[] allowedDoors = card.GetAllowedDirection();
                    cardDirectionIndicator.SetDoorIndiactor(allowedDoors);
                }

                // Farbe/Icons je nach roomtype
                if (HexImage != null && Frame != null)
                {
                    Color currentColor = new Color(0.3f, 0.3f, 0.3f); // Default

                    switch (card.roomtype)
                    {
                        case RoomType.Enemy:
                            currentColor = Color.red;
                            MonsterRoomIcon?.SetActive(true);
                            TreasureRoomIcon?.SetActive(false);
                            break;

                        case RoomType.NormalLott:
                            currentColor = Color.yellow;
                            MonsterRoomIcon?.SetActive(false);
                            TreasureRoomIcon?.SetActive(true);
                            break;

                        default:
                            MonsterRoomIcon?.SetActive(false);
                            TreasureRoomIcon?.SetActive(false);
                            break;
                    }

                    // Frame und HexImage färben
                    if (Frame.GetComponent<Image>() != null)
                        Frame.GetComponent<Image>().color = currentColor;
                    HexImage.color = currentColor;

                    // NEU: nameText und Icons einfärben
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

        // Hover
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (transform.parent == handPanel)
            {
                transform.localScale = hoverScale;
                transform.localRotation = Quaternion.identity;
                transform.localPosition += hoverOffset;

                if (tooltip != null)
                {
                    //tooltip.SetActive(true);
                    var tmp = tooltip.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmp != null && card != null)
                    {
                        tmp.text = card.cardDescription;
                    }

                    AdjustNeighborCards(true);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
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

        // Klick
        public void OnPointerClick(PointerEventData eventData)
        {
            // Hover rücksetzen
            transform.localScale = originalScale;
            transform.localPosition = originalPosition;
            transform.localRotation = originalRotation;
            HideTooltip();
            AdjustNeighborCards(false);

                     
            if (cardToHand != null)
            {
                cardToHand.OnCardClicked(this);
            }
            else
            {
                Debug.LogWarning("Keine cardToHand-Referenz in DisplayCard vorhanden!");
            }
        }

        private void AdjustNeighborCards(bool isHovering)
        {
            
            Transform parent = transform.parent;
            if (parent == null) return;

            int currentIndex = transform.GetSiblingIndex();

            // Linker Nachbar
            if (currentIndex > 0)
            {
                Transform leftNeighbor = parent.GetChild(currentIndex - 1);
                if (leftNeighbor.TryGetComponent<DisplayCard>(out var leftCard))
                {
                    Vector3 offset = isHovering ? -sideOffset : sideOffset;
                    leftNeighbor.localPosition += offset;
                }
            }

            // Rechter Nachbar
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
