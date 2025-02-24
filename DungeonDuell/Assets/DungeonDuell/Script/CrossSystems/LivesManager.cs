using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dungeonduell
{
    public class LivesManager : MonoBehaviour, IObserver
    {
        public int livesPlayer1 = 2;
        public int livesPlayer2 = 2;

        public bool nextRoundFinal = false;
        void Awake()
        {
            ConnectionsCollector[] objs = FindObjectsOfType<ConnectionsCollector>();

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }
        public void FinalRound()
        {
            nextRoundFinal = true;
            livesPlayer1 = 1;
            livesPlayer2 = 1;
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 1)
            {
                if (nextRoundFinal)
                {
                    SequenceMang sequenceMang;
                    if (sequenceMang = FindAnyObjectByType<SequenceMang>())
                    {
                        sequenceMang.DisableTimer();
                    }
                }
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
            DDCodeEventHandler.DungeonConnected += FinalRound;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void UnsubscribeToAllEvents()
        {
            DDCodeEventHandler.DungeonConnected -= FinalRound;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
