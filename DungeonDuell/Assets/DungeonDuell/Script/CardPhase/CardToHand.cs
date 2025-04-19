using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Collections;

namespace dungeonduell
{
    public class CardToHand : MonoBehaviour, IObserver
    {
        [Header("Deck / Player")] public PlayerDeck playerDeck;
        public int handLimit = 3;

        [Header("UI References")] public CardPrefabHolder cardPrefabHolder;
        public Transform handPanel;
        public Transform cardHolder;

        [Header("Fächer-Einstellungen")] public float spacing = 40f;
        public float maxRotation = 20f;

        public List<Card> handCards = new List<Card>();
        private readonly List<DisplayCard> _displayCards = new List<DisplayCard>();

        public bool isPlayer1;

        void Start()
        {
            DrawInitialCards();
        }

        private void DrawInitialCards()
        {
            if (playerDeck == null)
            {
                Debug.LogWarning("Kein PlayerDeck zugewiesen!");
                return;
            }

            handCards.Clear();

            for (int i = 0; i < handLimit; i++)
            {
                if (playerDeck.playerDeck.Count > 0)
                {
                    Card cardToDraw = playerDeck.playerDeck[0];
                    playerDeck.playerDeck.RemoveAt(0);
                    handCards.Add(cardToDraw);
                }
                else
                {
                    Debug.Log("Das Deck ist leer, keine weitere Karte kann gezogen werden!");
                    break;
                }
            }

            DisplayHand();
        }

        private void DisplayHand()
        {
            for (int i = handPanel.childCount - 1; i >= 0; i--)
            {
                Transform child = handPanel.GetChild(i);
                if (child == cardHolder) continue;
                Destroy(child.gameObject);
            }

            _displayCards.Clear();

            foreach (Card cardData in handCards)
            {
                GameObject cardObj = Instantiate(cardPrefabHolder.GetCardPrefab(cardData.roomtype), handPanel);
                DisplayCard dc = cardObj.GetComponent<DisplayCard>();

                dc.cardToHand = this;
                dc.card = cardData;
                _displayCards.Add(dc);
            }

            int n = _displayCards.Count;
            float middleIndex = (n - 1) / 2f;
            for (int i = 0; i < n; i++)
            {
                DisplayCard dc = _displayCards[i];
                float offsetFromCenter = i - middleIndex;
                float xPos = offsetFromCenter * spacing;
                float zRot = -offsetFromCenter * maxRotation;

                dc.transform.localPosition = new Vector2(xPos, 0f);
                dc.transform.localRotation = Quaternion.Euler(0f, 0f, zRot);
                dc.transform.localScale = Vector3.one;
            }

            SetupNavigation();
        }

        public void OnCardClicked(DisplayCard clickedCard)
        {
            if (clickedCard.transform.parent == cardHolder)
            {
                if (!handCards.Contains(clickedCard.card))
                {
                    handCards.Add(clickedCard.card);
                }

                clickedCard.transform.SetParent(handPanel, false);
                DDCodeEventHandler.Trigger_CardSelected(null);

                HexgridController hexgridController = FindFirstObjectByType<HexgridController>();
                if (hexgridController != null)
                {
                    hexgridController.ResetNavigation();
                }

                ReactivateHandCards();
                DisplayHand();
                SetupNavigation();

                //Select wieder aktiv setzen
                if (_displayCards.Count > 0)
                {
                    DisplayCard firstCard = _displayCards[^1]; // ganz rechts
                    if (firstCard != null && firstCard.GetComponent<Selectable>() is { } firstSel)
                    {
                        StartCoroutine(SetSelectableNextFrame(firstSel));
                    }
                }
            }
            else
            {
                handCards.Remove(clickedCard.card);

                if (cardHolder.childCount > 0)
                {
                    Transform oldCardTransform = cardHolder.GetChild(0);
                    DisplayCard oldDc = oldCardTransform.GetComponent<DisplayCard>();
                    if (oldDc != null)
                    {
                        if (!handCards.Contains(oldDc.card))
                            handCards.Add(oldDc.card);

                        oldDc.transform.SetParent(handPanel, false);
                    }
                }

                clickedCard.transform.SetParent(cardHolder, false);
                clickedCard.transform.localPosition = Vector3.zero;
                clickedCard.transform.localRotation = Quaternion.identity;
                clickedCard.transform.localScale = Vector3.one;

                DisableHandCardsForNavigation();
                DeactivateHandCards();
                EventSystem.current.SetSelectedGameObject(null);

                DDCodeEventHandler.Trigger_CardSelected(clickedCard);

                HexgridController hexgridController = FindFirstObjectByType<HexgridController>();
                if (hexgridController != null)
                {
                    hexgridController.currentDisplayCard = clickedCard;
                    hexgridController.ActivateNavigation();
                }

                DisplayHand();
                DeactivateHandCards(); // Jetzt auf die NEUEN Karten anwenden
            }
        }

        public void SetPlayerDeck(PlayerDeck deck)
        {
            playerDeck = deck;
        }


        public void DisableHandCardsForNavigation()
        {
            foreach (DisplayCard dc in _displayCards)
            {
                if (dc.transform.parent != cardHolder)
                {
                    dc.GetComponent<Selectable>().interactable = false;
                }
            }
        }

        private void EnableHandCardsForNavigation()
        {
            foreach (DisplayCard dc in _displayCards)
            {
                if (dc != null)
                {
                    Selectable selectable = dc.GetComponent<Selectable>();
                    if (selectable != null)
                    {
                        selectable.interactable = true;
                    }
                }
            }
        }

        public void DeactivateHandCards()
        {
            foreach (Transform child in handPanel)
            {
                if (child == cardHolder) continue;
                child.gameObject.SetActive(false);
            }
        }

        public void ReactivateHandCards()
        {
            foreach (Transform child in handPanel)
            {
                if (child == cardHolder) continue;
                child.gameObject.SetActive(true);
            }

            EnableHandCardsForNavigation();
            SetupNavigation();
        }

        public void FirstSelectable()
        {
            foreach (Transform child in handPanel)
            {
                if (child.TryGetComponent<Selectable>(out var sel) && sel.interactable &&
                    sel.gameObject.activeInHierarchy)
                {
                    EventSystem.current.SetSelectedGameObject(null); // Reset vorheriger Fokus
                    StartCoroutine(SetSelectableNextFrame(sel));
                    break; // Nur das erste gültige Objekt selektieren
                }
            }
        }

        private IEnumerator SetSelectableNextFrame(Selectable sel)
        {
            yield return null; // Einen Frame warten
            sel.Select(); // Jetzt wird das Event korrekt ausgelöst
        }

        private void SetupNavigation()
        {
            List<Selectable> selectables = new List<Selectable>();

            foreach (Card card in handCards)
            {
                DisplayCard displayCard = _displayCards.Find(dc => dc.card == card);

                if (displayCard != null)
                {
                    Selectable selectable = displayCard.GetComponent<Selectable>();
                    if (selectable != null)
                    {
                        selectables.Add(selectable);
                    }
                }
            }

            for (int i = 0; i < selectables.Count; i++)
            {
                Navigation nav = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = (i > 0) ? selectables[i - 1] : null,
                    selectOnRight = (i < selectables.Count - 1) ? selectables[i + 1] : null
                };

                selectables[i].navigation = nav;
            }

            if (selectables.Count > 0)
            {
                EventSystem.current.SetSelectedGameObject(selectables[0].gameObject);
            }
        }

        public void DrawCard()
        {
            if (playerDeck != null && playerDeck.playerDeck.Count > 0)
            {
                if (handCards.Count < handLimit)
                {
                    Card cardToDraw = playerDeck.playerDeck[0];
                    playerDeck.playerDeck.RemoveAt(0);
                    handCards.Add(cardToDraw);

                    DisplayHand();
                }
            }
        }

        private void UpdateCardDeck(Card card, bool player1Played)
        {
            if (player1Played == isPlayer1)
            {
                handCards.Remove(card);
                if (handCards.Count == 0)
                {
                    DDCodeEventHandler.Trigger_PlayedAllCards(isPlayer1);
                }
            }
        }

        public void ShowHideDeck(bool hide)
        {
            if (hide)
            {
                transform.DOMoveY(-300, 0.5f).OnComplete(() => { gameObject.SetActive(false); });
            }
            else
            {
                gameObject.SetActive(true);

                Vector3 startPos = transform.position;
                startPos.y = -300;
                transform.position = startPos;

                transform.DOMoveY(0, 0.5f);
            }
        }

        void OnEnable()
        {
            SubscribeToEvents();
        }

        void OnDisable()
        {
            UnsubscribeToAllEvents();
        }

        public void SubscribeToEvents()
        {
            DDCodeEventHandler.CardPlayed += UpdateCardDeck;
            DDCodeEventHandler.CardToBeShelled +=
                UpdateCardDeck; // When shell card played is modified (and .remove(card) will do nothing) so extra call before that happens
        }

        public void UnsubscribeToAllEvents()
        {
            DDCodeEventHandler.CardPlayed -= UpdateCardDeck;
            DDCodeEventHandler.CardToBeShelled -= UpdateCardDeck;
        }
    }
}