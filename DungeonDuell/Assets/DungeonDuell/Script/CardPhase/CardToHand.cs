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

        // Karten, die der Spieler gerade in der Hand hält
        private List<Card> handCards = new List<Card>();

        // Verweise auf die DisplayCard-Objekte in der Hand
        private List<DisplayCard> displayCards = new List<DisplayCard>();

        void Start()
        {
            // Beispiel: Wir ziehen direkt am Start "handLimit" Karten aus playerDeck
            DrawInitialCards();
        }

        /// <summary>
        /// Zieht anfänglich bis zu "handLimit" Karten aus dem "playerDeck.playerDeck"
        /// und zeigt sie in der Hand an.
        /// </summary>
        private void DrawInitialCards()
        {
            // Safety-Check
            if (playerDeck == null)
            {
                Debug.LogWarning("Kein PlayerDeck zugewiesen!");
                return;
            }

            handCards.Clear();

            // Ziehe so viele Karten aus "playerDeck.playerDeck", wie handLimit erlaubt
            for (int i = 0; i < handLimit; i++)
            {
                if (playerDeck.playerDeck.Count > 0)
                {
                    // Nimm die erste Karte (oder zufällige Karte, je nach Wunsch)
                    Card cardToDraw = playerDeck.playerDeck[0];
                    // Entferne sie aus dem Deck
                    playerDeck.playerDeck.RemoveAt(0);
                    // Füge sie der Hand hinzu
                    handCards.Add(cardToDraw);
                }
                else
                {
                    Debug.Log("Das Deck ist leer, keine weitere Karte kann gezogen werden!");
                    break;
                }
            }

            // Zeige die Karten in der Hand
            DisplayHand();
        }

        /// <summary>
        /// Zeigt alle Karten in der Hand an (erzeugt DisplayCard-Objekte und positioniert sie).
        /// </summary>
        public void DisplayHand()
        {
            // 1) Alte Hand-Karten löschen
            for (int i = handPanel.childCount - 1; i >= 0; i--)
            {
                Transform child = handPanel.GetChild(i);

                // --- Skip, wenn es der cardHolder selbst ist ---
                if (child == cardHolder)
                {
                    // cardHolder NICHT löschen!
                    continue;
                }

                // Sonst: Das ist eine alte Karte => weg damit
                Destroy(child.gameObject);
            }

            // Clear-Logik für displayCards-Liste
            displayCards.Clear();

            // 2) Neue UI-Karten erzeugen
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


            // Fächer-Layout (simple lineare Anordnung)
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

        /// <summary>
        /// Aufruf von DisplayCard, wenn geklickt wird. 
        /// Verschiebt die Karte zwischen Hand und CardHolder und feuert ggf. Events.
        /// </summary>
        public void OnCardClicked(DisplayCard clickedCard)
        {
            // **1) Karte war schon im CardHolder** → Spieler legt sie zurück in die Hand
            if (clickedCard.transform.parent == cardHolder)
            {
                // Füge die Karte wieder den Handkarten hinzu (falls sie fehlt)
                if (!handCards.Contains(clickedCard.card))
                {
                    handCards.Add(clickedCard.card);
                }

                // Verschiebe sie optisch ins HandPanel
                clickedCard.transform.SetParent(handPanel, false);

                // Event: Keine Karte mehr ausgewählt
                DDCodeEventHandler.Trigger_CardSelected(null);

                // **Handkarten wieder aktivieren** (Selectables true)
                EnableHandCardsForNavigation();

                // Hand-Layout aktualisieren
                DisplayHand();

                // Da wir jetzt wieder in der Hand sind, 
                // Navigation für UI-Karten neu aufbauen (explizit links/rechts)
                SetupNavigation(skipButton);

                // **Fokus sofort setzen** – z. B. auf die erste Karte
                if (displayCards.Count > 0)
                {
                    Selectable firstCardSel = displayCards[0].GetComponent<Selectable>();
                    if (firstCardSel != null)
                    {
                        EventSystem.current.SetSelectedGameObject(firstCardSel.gameObject);
                    }
                }
            }
            // **2) Karte lag in der Hand** → Spieler legt sie in den CardHolder
            else
            {
                // Entferne die Karte aus der Handliste
                handCards.Remove(clickedCard.card);

                // OPTIONAL: Falls bereits eine andere Karte im Holder liegt, 
                // verschiebe die alte zurück in die Hand
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

                // **Verschiebe die geklickte Karte in den CardHolder**
                clickedCard.transform.SetParent(cardHolder, false);
                clickedCard.transform.localPosition = Vector3.zero;
                clickedCard.transform.localRotation = Quaternion.identity;
                clickedCard.transform.localScale = Vector3.one;

                // **Handkarten‐Selectables deaktivieren**, damit man nicht mehr 
                // gleichzeitig in Hand & Hex navigieren kann
                DisableHandCardsForNavigation();

                // **UI‐Fokus rausnehmen**, damit nicht versehentlich noch eine Karte selektiert ist
                EventSystem.current.SetSelectedGameObject(null);

                // Event: Neue Karte ausgewählt
                DDCodeEventHandler.Trigger_CardSelected(clickedCard);

                // Hexgrid‐Navigation aktivieren
                HexgridController hexgridController = FindObjectOfType<HexgridController>();
                if (hexgridController != null)
                {
                    hexgridController.currentDisplayCard = clickedCard; // merk dir, welche Karte platziert werden soll
                    hexgridController.ActivateNavigation();
                }

                // Hand-Layout aktualisieren (damit die Karte nicht mehr in der Hand angezeigt wird)
                DisplayHand();
            }
        }


        private void DisableHandCardsForNavigation()
        {
            foreach (DisplayCard dc in displayCards)
            {
                if (dc.transform.parent != cardHolder) 
                {
                    dc.GetComponent<Selectable>().interactable = false;
                }
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

        public bool HasCardOnHolder()
        {
            return cardHolder.childCount > 0; // Gibt true zurück, wenn mindestens eine Karte im CardHolder liegt
        }


        public void SelectFirstCard()
        {
            List<Selectable> selectables = new List<Selectable>();

            // Handkarten in richtiger Reihenfolge durchgehen
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

            // Falls keine Handkarten da sind → Skip-Button auswählen
            if (selectables.Count == 0 && skipButton != null)
            {
                EventSystem.current.SetSelectedGameObject(skipButton.gameObject);
                return;
            }

            // Erste Karte auswählen, wenn vorhanden
            if (selectables.Count > 0)
            {
                EventSystem.current.SetSelectedGameObject(selectables[0].gameObject);
            }

            SetupNavigation(skipButton);
        }

        public void SetupNavigation(Selectable skipButton)
        {
            List<Selectable> selectables = new List<Selectable>();

            // Handkarten in richtiger Reihenfolge durchgehen
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

            // Skip-Button am Ende hinzufügen
            if (skipButton != null)
            {
                selectables.Add(skipButton);
            }

            // Setze die Navigation explizit für alle Karten
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

            // Erste Karte oder Skip-Button auswählen
            if (selectables.Count > 0)
            {
                EventSystem.current.SetSelectedGameObject(selectables[0].gameObject);
            }
        }

        public void DrawCard()
        {
            if (playerDeck != null && playerDeck.playerDeck.Count > 0)
            {
                // Nur ziehen, wenn Platz
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
                // 1) Fahre das Objekt nach unten (Y=-300) in 0.5s.
                //    Wenn Animation fertig ist, deaktiviere das Objekt.
                transform.DOMoveY(-300, 0.5f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            }
            else
            {
                // 2) Aktiviere das Objekt zuerst
                gameObject.SetActive(true);

                //    Setze die Startposition auf Y=-300 (z. B. außerhalb des Sichtbereichs)
                //    und fahre es hoch auf Y=0 in 0.5s.
                Vector3 startPos = transform.position;
                startPos.y = -300; // oder irgendein Wert, wo es „unsichtbar“ ist
                transform.position = startPos;

                transform.DOMoveY(0, 0.5f);
            }
        }
    }
}
