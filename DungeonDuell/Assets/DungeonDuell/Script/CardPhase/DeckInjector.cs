using System.Collections;
using UnityEngine;

namespace dungeonduell
{
    public class DeckInjector : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(LateInject());
        }

        private IEnumerator LateInject()
        {
            yield return null; // Warte ein Frame

            var allDecks = FindObjectsByType<PlayerDeck>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            var allHands = FindObjectsByType<CardToHand>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var hand in allHands)
            foreach (var deck in allDecks)
                if (hand.isPlayer1 == deck.isPlayerOne)
                    hand.SetPlayerDeck(deck);
        }
    }
}