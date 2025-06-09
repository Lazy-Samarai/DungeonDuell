using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dungeonduell
{
    public class PlayerDeck : MonoBehaviour
    {
        public int designatedCardScene;

        // Liste der verf�gbaren Karten. Mit allen ScriptableObjectCards im Editor f�llen
        public bool useDistributorSystem = true;

        public bool firstTime = true;

        public List<CardDistributor> cardDistributors = new();

        public List<Card> availableCards = new();

        public List<Card> playerDeck = new();

        public int deckSize = 3;

        public bool isPlayerOne; // Im Inspector setzen


        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == designatedCardScene) GenerateRandomDeck();
        }

        private void GenerateRandomDeck()
        {
            if (firstTime)
            {
                GetPerDistributer();
            }

            if (availableCards.Count == 0)
            {
                Debug.LogError("Die Liste der verf�gbaren Karten ist leer!");
                return;
            }

            playerDeck.Clear();


            PickCards();
            if (firstTime)
            {
                // If no Enemy Card on First time replace one with it
                if (playerDeck.Find(card => card.roomtype == RoomType.Enemy) == null)
                {
                    var randomIndex = Random.Range(0, playerDeck.Count);
                    availableCards.Add(playerDeck[randomIndex]);
                    playerDeck.RemoveAt(randomIndex);
                    playerDeck.Add(availableCards.Find(card => card.roomtype == RoomType.Enemy));
                }

                firstTime = false;
            }

            if (availableCards.Count <= 0) GetPerDistributer();
        }

        private void PickCards()
        {
            for (var i = 0; i < deckSize; i++)
            {
                var randomIndex = Random.Range(0, firstTime ? availableCards.Count - 1 : availableCards.Count);
                playerDeck.Add(availableCards[randomIndex]);
                availableCards.RemoveAt(randomIndex);
            }
        }

        private void GetPerDistributer()
        {
            availableCards.Clear();
            foreach (var cardDistributor in cardDistributors)
                for (var i = 0; i < cardDistributor.Amount; i++)
                    availableCards.Add(cardDistributor.Card);
        }
    }
}