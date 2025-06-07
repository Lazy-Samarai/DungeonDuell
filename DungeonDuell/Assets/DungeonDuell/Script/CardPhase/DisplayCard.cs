using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

        [Header("Hover-Effekt")] public Vector3 hoverScale = new(1.2f, 1.2f, 1f);
        public Vector3 hoverOffset = new(0f, 20f, 0f);
        public Vector3 sideOffset = new(30f, 0f, 0f);


        [SerializeField] private Sprite[] spritesFullCard;
        private Vector3 _originalLeftPosition;
        private Vector3 _originalPosition;
        private Vector3 _originalRightPosition;
        private Quaternion _originalRotation;

        private Vector3 _originalScale;

        private Selectable _selectable;

        private void Start()
        {
            _selectable = GetComponent<Selectable>();
            if (handPanel == null)
            {
                var cth = GetComponentInParent<CardToHand>();
                if (cth != null)
                    handPanel = cth.handPanel;
                else
                    Debug.LogError("Kein CardToHand im Parent gefunden. HandPanel kann nicht ermittelt werden!");
            }

            _originalScale = transform.localScale;
            _originalPosition = transform.localPosition;
            _originalRotation = transform.localRotation;


            var parent = transform.parent;

            if (parent != null)
            {
                if (_selectable.navigation.selectOnLeft != null)
                {
                    var leftNeighbor = _selectable.navigation.selectOnLeft.transform;
                    _originalLeftPosition = leftNeighbor.localPosition;
                }

                if (_selectable.navigation.selectOnRight != null)
                {
                    var rightNeighbor = _selectable.navigation.selectOnRight.transform;
                    _originalRightPosition = rightNeighbor.localPosition;
                }
            }

            UpdateDirectionIndicator(card.GetAllowedDirection());
            HideTooltip();
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
                cardToHand.OnCardClicked(this);
            else
                Debug.LogWarning("Keine cardToHand-Referenz in DisplayCard vorhanden!");
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

        public void UpdateDirectionIndicator(bool[] allowedDoors)
        {
            cardDirectionIndicator1.SetDoorIndiactor(allowedDoors);
            cardDirectionIndicator2.SetDoorIndiactor(allowedDoors);
        }

        private void ActivateHoverEffect()
        {
            transform.SetSiblingIndex(transform.parent.childCount);
            if (transform.parent == handPanel)
            {
                transform.localScale = hoverScale;
                transform.localRotation = Quaternion.identity;
                transform.localPosition += hoverOffset;

                if (tooltip != null)
                {
                    var tmp = tooltip.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmp != null && card != null) tmp.text = card.cardDescription;

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
            var parent = transform.parent;
            if (parent == null) return;

            if (_selectable.navigation.selectOnLeft != null)
            {
                var leftNeighbor = _selectable.navigation.selectOnLeft.transform;
                if (leftNeighbor.TryGetComponent<DisplayCard>(out _))
                {
                    if (isHovering)
                        leftNeighbor.localPosition = _originalLeftPosition - sideOffset;
                    else
                        leftNeighbor.localPosition = _originalLeftPosition;
                }
            }

            if (_selectable.navigation.selectOnRight != null)
            {
                var rightNeighbor = _selectable.navigation.selectOnRight.transform;
                if (rightNeighbor.TryGetComponent<DisplayCard>(out _))
                {
                    if (isHovering)
                        rightNeighbor.localPosition = _originalRightPosition + sideOffset;
                    else
                        rightNeighbor.localPosition = _originalRightPosition;
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