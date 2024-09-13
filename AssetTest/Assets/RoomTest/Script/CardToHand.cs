using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class CardToHand : MonoBehaviour
    {
        public List<Card> handCards = new List<Card>(); // Liste der Karten in der Hand
        public GameObject cardPrefab; // Prefab für die Handkarten (falls du ein UI-Element dafür hast)
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

            for (int i = 0; i < handCards.Count; i++)
            {
                // Kartenposition und Rotation für den Fächer-Effekt
                float angle = Mathf.Lerp(-spreadAngle, spreadAngle, i / cardCount);
                Vector3 positionOffset = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * handRadius, 0, Mathf.Cos(angle * Mathf.Deg2Rad) * handRadius);

                // Korrigiere die Drehung der Karten
                float rotationAngle = -angle;

                DisplayCardInHand(handCards[i], positionOffset, rotationAngle);
            }
        }



        // Einzelne Karte anzeigen
        void DisplayCardInHand(Card card, Vector3 positionOffset, float angle)
        {
            if (cardPrefab != null && handPanel != null)
            {
                GameObject cardObject = Instantiate(cardPrefab, handPanel);
                DisplayCard cardDisplay = cardObject.GetComponent<DisplayCard>();

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
    }
}
