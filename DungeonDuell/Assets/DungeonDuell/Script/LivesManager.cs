using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace dungeonduell
{
    public class LivesManager : MonoBehaviour
    {
        public int livesPlayer1 = 2;
        public int livesPlayer2 = 2;

        bool nextRoundFinal = false;
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

        void OnEnable()
        {
            if(nextRoundFinal){
                
                SequenceMang sequenceMang; // A bit over simple but fine for now
                if(sequenceMang = FindAnyObjectByType<SequenceMang>())
                {
                    sequenceMang.DisableTimer();
                }
            }
        
            DDCodeEventHandler.DungeonConnected+=FinalRound;
        }
        void OnDisable()
        {
             DDCodeEventHandler.DungeonConnected-=FinalRound;
        }
    }
}
