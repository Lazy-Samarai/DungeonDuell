using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dungeonduell
{
    public class LivesManager : MonoBehaviour
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
        void OnSceneLoaded(Scene scene, LoadSceneMode mode){
            if(scene.buildIndex == 1){
                    if(nextRoundFinal){
                
                    SequenceMang sequenceMang; // A bit over simple but fine for now
                    if(sequenceMang = FindAnyObjectByType<SequenceMang>())
                    {
                        sequenceMang.DisableTimer();
                    }

                }
            }

        }


        void OnEnable()
        {
            
        
            DDCodeEventHandler.DungeonConnected+=FinalRound;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void OnDisable()
        {
            DDCodeEventHandler.DungeonConnected-=FinalRound;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
