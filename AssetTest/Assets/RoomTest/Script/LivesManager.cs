using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class LivesManager : MonoBehaviour
    {
        public int livesPlayer1 = 2;
        public int livesPlayer2 = 2;

        void Awake()
        {
            ConnectionsCollector[] objs = FindObjectsOfType<ConnectionsCollector>();

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}
