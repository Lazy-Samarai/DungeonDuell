using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace dungeonduell
{
    public class CardToHand : MonoBehaviour
    {
        public List<Card> handCards = new List<Card>(); // Liste der Karten in der Hand
        public GameObject cardPrefab; // Prefab für die Handkarten (muss ein Selectable besitzen, z.B. Button)
        public Transform handPanel; // UI-Panel, in dem die Handkarten angezeigt werden
        public int handLimit = 3;
        public PlayerDeck playerDeck;
        public float spreadAngle = 30f; // Maximaler Winkel für den Fächer
        public float handRadius = 200f; // Abstand der Karten zum Mittelpunkt

        void Start()
        {
            if (handPanel == null)
            {
                Debug.LogError("HandPanel ist nicht zugewiesen!");
                return;
            }

            if (cardPrefab == null)
            {
                Debug.LogError("CardPrefab ist nicht zugewiesen!");
                return;
            }

            DrawInitialCards();
        }

        void DrawInitialCards()
        {
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
                    Debug.LogWarning("Nicht genug Karten im Deck!");
                    break;
                }
            }

            DisplayHand();
        }

        void DisplayHand()
        {
            float cardCount = Mathf.Max(handCards.Count - 1, 1);

            // (1) Erzeuge die Karten in einem Schleifendurchlauf
            for (int i = 0; i < handCards.Count; i++)
            {
                float angle = Mathf.Lerp(-spreadAngle, spreadAngle, i / cardCount);
                Vector3 positionOffset = new Vector3(
                    Mathf.Sin(angle * Mathf.Deg2Rad) * handRadius,
                    0,
                    Mathf.Cos(angle * Mathf.Deg2Rad) * handRadius
                );
                float rotationAngle = -angle;

                DisplayCardInHand(handCards[i], positionOffset, rotationAngle);
            }

            // (2) Suche im HandPanel nach dem ersten Child mit DisplayCard
            for (int i = 0; i < handPanel.childCount; i++)
            {
                DisplayCard cardScript = handPanel.GetChild(i).GetComponent<DisplayCard>();
                if (cardScript != null)  // => Ja, wir haben eine Karte
                {
                    // EventSystem-Fokus setzen
                    EventSystem.current.SetSelectedGameObject(handPanel.GetChild(i).gameObject);
                    break; // Erstes gefundenes Child fokussieren und abbrechen
                }
            }
        }


        // Einzelne Karte anzeigen
        void DisplayCardInHand(Card card, Vector3 positionOffset, float angle)
        {
            if (cardPrefab != null && handPanel != null)
            {
                GameObject cardObject = Instantiate(cardPrefab, handPanel);
                DisplayCard cardDisplay = cardObject.GetComponent<DisplayCard>();
                cardDisplay.cardHolder = transform.GetChild(0).gameObject;

                if (cardDisplay != null)
                {
                    cardDisplay.card = card;
                    cardDisplay.UpdateCardDisplay();

                    // Position und Rotation setzen
                    cardObject.transform.localPosition = positionOffset;
                    cardObject.transform.localRotation = Quaternion.Euler(0, 0, angle);
                    cardObject.transform.localScale = Vector3.one;
                }
            }
            else
            {
                Debug.LogError("cardPrefab oder handPanel ist nicht zugewiesen!");
            }
        }

        // Methode zum Ziehen einer Karte
        public void DrawCard()
        {
            if (playerDeck.playerDeck.Count > 0)
            {
                Card cardToDraw = playerDeck.playerDeck[0];
                playerDeck.playerDeck.RemoveAt(0);
                handCards.Add(cardToDraw);

                DisplayHand();
            }
            else
            {
                Debug.LogWarning("Das Deck ist leer, keine Karte kann gezogen werden!");
            }
        }

        public void ShowHideDeck(bool hide)
        {
            if (hide)
            {
                // Setze sofort auf deaktiviert und führe die Animation aus
                gameObject.SetActive(false); // Deaktiviert das GameObject direkt

                // Fade Out und Slide Out Animation (falls das Objekt aktiviert ist)
                transform.DOMoveY(-300, 0.5f).Play();
            }
            else
            {
                // Set Active und Animation für Slide In
                gameObject.SetActive(true); // Aktiviert das GameObject direkt

                // Setze die Startposition und führe die Einblend-Animation aus
                transform.position = new Vector3(transform.position.x, -300, transform.position.z);
                transform.DOMoveY(0.25f, 0.5f).Play();
            }
        }
    }
}
