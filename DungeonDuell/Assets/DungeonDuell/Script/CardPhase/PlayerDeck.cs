using System.Collections.Generic;
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
                firstTime = false;
            }

            if (availableCards.Count == 0)
            {
                Debug.LogError("Die Liste der verf�gbaren Karten ist leer!");
                return;
            }

            playerDeck.Clear();

            for (var i = 0; i < deckSize; i++)
            {
                var randomIndex = Random.Range(0, availableCards.Count);
                playerDeck.Add(availableCards[randomIndex]);
                availableCards.RemoveAt(randomIndex);


                if (availableCards.Count <= 0) GetPerDistributer();
            }
        }

        private void GetPerDistributer()
        {
            availableCards.Clear();
            foreach (var CardDistributor in cardDistributors)
                for (var i = 0; i < CardDistributor.Amount; i++)
                    availableCards.Add(CardDistributor.Card);
        }
    }
}