using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace dungeonduell
{
    public class DisplayCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,
        ISelectHandler, IDeselectHandler
    {
        public Card card;
        public CardToHand cardToHand;
        public Transform handPanel;

        [Header("UI References")] public TextMeshProUGUI nameText;
        public DoorIndicator cardDirectionIndicator1;
        public DoorIndicator cardDirectionIndicator2;
        public GameObject tooltip;

        [Header("Hover-Effekt")] public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f);
        public Vector3 hoverOffset = new Vector3(0f, 20f, 0f);
        public Vector3 sideOffset = new Vector3(30f, 0f, 0f);

        private Vector3 _originalScale;
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private Vector3 _originalLeftPosition;
        private Vector3 _originalRightPosition;


        [SerializeField] private Sprite[] spritesFullCard;

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

            _originalScale = transform.localScale;
            _originalPosition = transform.localPosition;
            _originalRotation = transform.localRotation;

            int index = transform.GetSiblingIndex();
            Transform parent = transform.parent;

            if (parent != null)
            {
                if (index > 0)
                {
                    Transform leftNeighbor = parent.GetChild(index - 1);
                    _originalLeftPosition = leftNeighbor.localPosition;
                }

                if (index < parent.childCount - 1)
                {
                    Transform rightNeighbor = parent.GetChild(index + 1);
                    _originalRightPosition = rightNeighbor.localPosition;
                }
            }

            UpdateDirectionIndicator(card.GetAllowedDirection());
            HideTooltip();
        }

        public void UpdateDirectionIndicator(bool[] allowedDoors)
        {
            cardDirectionIndicator1.SetDoorIndiactor(allowedDoors);
            cardDirectionIndicator2.SetDoorIndiactor(allowedDoors);
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

        private void ActivateHoverEffect()
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
                transform.localScale = _originalScale;
                transform.localRotation = _originalRotation;
                transform.localPosition = _originalPosition;

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
                if (leftNeighbor.TryGetComponent<DisplayCard>(out _))
                {
                    if (isHovering)
                    {
                        leftNeighbor.localPosition = _originalLeftPosition - sideOffset;
                    }
                    else
                    {
                        leftNeighbor.localPosition = _originalLeftPosition;
                    }
                }
            }

            if (currentIndex < parent.childCount - 1)
            {
                Transform rightNeighbor = parent.GetChild(currentIndex + 1);
                if (rightNeighbor.TryGetComponent<DisplayCard>(out _))
                {
                    if (isHovering)
                    {
                        rightNeighbor.localPosition = _originalRightPosition + sideOffset;
                    }
                    else
                    {
                        rightNeighbor.localPosition = _originalRightPosition;
                    }
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