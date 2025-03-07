using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace dungeonduell
{
    public class EnemyInRoomHandling : MonoBehaviour
    {
        public UnityEvent onAllEnemyDead;
        public int enemyCount = 0;
        public void AddEnemy()
        {
            enemyCount++;
        }
        public void EnemyDied()
        {
            enemyCount--;
            if(enemyCount <= 0)
            {
                onAllEnemyDead.Invoke();
            }
        }
    }
}
