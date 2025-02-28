using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dungeonduell
{
    public class PlayerDeck : MonoBehaviour
    {
        public int designatedCardScene = 0;
        // Liste der verf�gbaren Karten. Mit allen ScriptableObjectCards im Editor f�llen
        public bool useDistributorSystem = true;

        public bool firstTime = true;

        public List<CardDistributor> cardDistributors = new List<CardDistributor>();

        public List<Card> availableCards = new List<Card>();

        public List<Card> playerDeck = new List<Card>();

        public int deckSize = 3;


        void OnEnable()
        {        
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == designatedCardScene)
            {
                GenerateRandomDeck();
            }
        }

        void GenerateRandomDeck()
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

            // Zuf�llig Karten ausw�hlen und dem Deck hinzuf�gen 
            for (int i = 0; i < deckSize; i++)
            {
                int randomIndex = Random.Range(0, availableCards.Count);
                playerDeck.Add(availableCards[randomIndex]);
                availableCards.RemoveAt(randomIndex);


                if (availableCards.Count <= 0)
                {
                    GetPerDistributer();
                }

            }
        }

        private void GetPerDistributer()
        {
            availableCards.Clear();
            foreach (var CardDistributor in cardDistributors)
            {
                for (int i = 0; i < CardDistributor.Amount; i++)
                {
                    availableCards.Add(CardDistributor.Card);
                }
            }
        }
    }
}
