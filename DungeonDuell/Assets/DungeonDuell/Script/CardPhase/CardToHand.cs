using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace dungeonduell
{
    public class CardToHand : MonoBehaviour
    {
        [Header("Deck / Player")]
        public PlayerDeck playerDeck;
        public int handLimit = 3;
        public bool isPlayerOne;

        [Header("UI References")]
        public GameObject cardPrefab;
        public Transform handPanel;
        public Transform cardHolder;
        public Selectable skipButton;

        [Header("Fächer-Einstellungen")]
        public float spacing = 40f;
        public float maxRotation = 20f;

        private List<Card> handCards = new List<Card>();
        private List<DisplayCard> displayCards = new List<DisplayCard>();

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

        public void DisplayHand()
        {
            for (int i = handPanel.childCount - 1; i >= 0; i--)
            {
                Transform child = handPanel.GetChild(i);
                if (child == cardHolder) continue;
                Destroy(child.gameObject);
            }

            displayCards.Clear();

            for (int i = 0; i < handCards.Count; i++)
            {
                Card cardData = handCards[i];
                GameObject cardObj = Instantiate(cardPrefab, handPanel);
                DisplayCard dc = cardObj.GetComponent<DisplayCard>();

                dc.cardToHand = this;
                dc.card = cardData;
                dc.UpdateCardDisplay();

                displayCards.Add(dc);
            }

            int n = displayCards.Count;
            float middleIndex = (n - 1) / 2f;
            for (int i = 0; i < n; i++)
            {
                DisplayCard dc = displayCards[i];
                float offsetFromCenter = i - middleIndex;
                float xPos = offsetFromCenter * spacing;
                float zRot = -offsetFromCenter * maxRotation;

                dc.transform.localPosition = new Vector2(xPos, 0f);
                dc.transform.localRotation = Quaternion.Euler(0f, 0f, zRot);
                dc.transform.localScale = Vector3.one;
            }

            SetupNavigation(skipButton);
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

                HexgridController hexgridController = FindObjectOfType<HexgridController>();
                if (hexgridController != null)
                {
                    hexgridController.ResetNavigation();
                }

                ReactivateHandCards();
                DisplayHand();
                SetupNavigation(skipButton);

                if (displayCards.Count > 0)
                {
                    Selectable firstCardSel = displayCards[0].GetComponent<Selectable>();
                    if (firstCardSel != null)
                    {
                        EventSystem.current.SetSelectedGameObject(firstCardSel.gameObject);
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

                HexgridController hexgridController = FindObjectOfType<HexgridController>();
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
            foreach (DisplayCard dc in displayCards)
            {
                if (dc.transform.parent != cardHolder)
                {
                    dc.GetComponent<Selectable>().interactable = false;
                }
            }

            if (skipButton != null)
            {
                skipButton.interactable = false;
            }
        }

        public void EnableHandCardsForNavigation()
        {
            foreach (DisplayCard dc in displayCards)
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
            SetupNavigation(skipButton);
        }

        public bool HasCardOnHolder()
        {
            return cardHolder.childCount > 0;
        }

        public void SelectFirstCard()
        {
            List<Selectable> selectables = new List<Selectable>();

            foreach (Card card in handCards)
            {
                DisplayCard displayCard = displayCards.Find(dc => dc.card == card);

                if (displayCard != null)
                {
                    Selectable selectable = displayCard.GetComponent<Selectable>();
                    if (selectable != null)
                    {
                        selectables.Add(selectable);
                    }
                }
            }

            if (selectables.Count == 0 && skipButton != null)
            {
                EventSystem.current.SetSelectedGameObject(skipButton.gameObject);
                return;
            }

            if (selectables.Count > 0)
            {
                EventSystem.current.SetSelectedGameObject(selectables[0].gameObject);
            }

            SetupNavigation(skipButton);
        }

        public void SetupNavigation(Selectable skipButton)
        {
            List<Selectable> selectables = new List<Selectable>();

            foreach (Card card in handCards)
            {
                DisplayCard displayCard = displayCards.Find(dc => dc.card == card);

                if (displayCard != null)
                {
                    Selectable selectable = displayCard.GetComponent<Selectable>();
                    if (selectable != null)
                    {
                        selectables.Add(selectable);
                    }
                }
            }

            if (skipButton != null)
            {
                selectables.Add(skipButton);
            }

            for (int i = 0; i < selectables.Count; i++)
            {
                Navigation nav = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = (i > 0) ? selectables[i - 1] : null,
                    selectOnRight = (i < selectables.Count - 1) ? selectables[i + 1] : skipButton
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

        public void ShowHideDeck(bool hide)
        {
            if (hide)
            {
                transform.DOMoveY(-300, 0.5f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
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
    }
}
