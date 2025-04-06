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

            var allDecks = FindObjectsOfType<PlayerDeck>(true);
            var allHands = FindObjectsOfType<CardToHand>(true);

            foreach (var hand in allHands)
            {
                foreach (var deck in allDecks)
                {
                    if (hand.isPlayerOne == deck.isPlayerOne)
                    {
                        hand.SetPlayerDeck(deck);
                    }
                }
            }
        }
    }
}
