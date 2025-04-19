using UnityEngine;
using System.Collections;

namespace dungeonduell
{
    public class DeckInjector : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(LateInject());
        }

        IEnumerator LateInject()
        {
            yield return null; // Warte ein Frame

            var allDecks = FindObjectsByType<PlayerDeck>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            var allHands = FindObjectsByType<CardToHand>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var hand in allHands)
            {
                foreach (var deck in allDecks)
                {
                    if (hand.isPlayer1 == deck.isPlayerOne)
                    {
                        hand.SetPlayerDeck(deck);
                    }
                }
            }
        }
    }
}