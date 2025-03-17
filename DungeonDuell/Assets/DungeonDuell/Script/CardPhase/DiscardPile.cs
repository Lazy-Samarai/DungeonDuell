using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class DiscardPile : MonoBehaviour,IObserver
    {
        public List<Card> discardPile = new List<Card>();

        public bool belongPlayer1;

        // Methode zum Hinzuf�gen einer Karte zum Abwurfstapel
        public void AddCardToDiscardPile(Card card,bool Player1played)
        {
            if(Player1played == belongPlayer1)
            {
                discardPile.Add(card);
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
            DDCodeEventHandler.CardPlayed += AddCardToDiscardPile;
        }

        public void UnsubscribeToAllEvents()
        {
            DDCodeEventHandler.CardPlayed -= AddCardToDiscardPile;
        }
    }
}
