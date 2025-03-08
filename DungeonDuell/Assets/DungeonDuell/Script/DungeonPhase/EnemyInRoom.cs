using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace dungeonduell
{
    public class EnemyInRoom : MonoBehaviour
    {
        EnemyInRoomHandling enemyInRoomHandling;
        // Start is called before the first frame update
        void Start()
        {
            enemyInRoomHandling = GetComponentInParent<EnemyInRoomHandling>();
            if (enemyInRoomHandling != null) // if false Enemy Did not Spawn in a Enemy Room
            {
                enemyInRoomHandling.AddEnemy();
            }
        }
        public void Death()
        {
            if (enemyInRoomHandling != null)
            {
                enemyInRoomHandling.EnemyDied();
            }

        }
    }
}
