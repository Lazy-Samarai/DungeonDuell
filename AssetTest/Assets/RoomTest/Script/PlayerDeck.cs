using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class PlayerDeck : MonoBehaviour
    {
        // Liste der verfügbaren Karten. Mit allen ScriptableObjectCards im Editor füllen
        public List<Card> availableCards = new List<Card>();

        public List<Card> playerDeck = new List<Card>();

        public int deckSize = 20;

        void Awake()
        {
            
            GenerateRandomDeck();
        }


        // Erstellen eines Decks mit einer festgelegten Anzahl von Karten
        void GenerateRandomDeck()
        {
           
            if (availableCards.Count == 0)
            {
                Debug.LogError("Die Liste der verfügbaren Karten ist leer!");
                return;
            }

            playerDeck.Clear();

            // Zufällig Karten auswählen und dem Deck hinzufügen 
            for (int i = 0; i < deckSize; i++)
            {
                int randomIndex = Random.Range(0, availableCards.Count); 
                playerDeck.Add(availableCards[randomIndex]); 
            }

            Debug.Log("Deck erfolgreich generiert mit " + playerDeck.Count + " Karten.");
        }
    }
}
