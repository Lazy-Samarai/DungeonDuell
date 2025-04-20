using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class DiscardPile : MonoBehaviour, IObserver
    {
        public List<Card> discardPile = new();

        public bool belongPlayer1;

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToAllEvents();
        }

        public void SubscribeToEvents()
        {
            DdCodeEventHandler.CardPlayed += AddCardToDiscardPile;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.CardPlayed -= AddCardToDiscardPile;
        }

        // Methode zum Hinzuf�gen einer Karte zum Abwurfstapel
        public void AddCardToDiscardPile(Card card, bool player1Played)
        {
            if (player1Played == belongPlayer1) discardPile.Add(card);
        }
    }
}