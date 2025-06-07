using UnityEngine;
using UnityEngine.Events;

namespace dungeonduell
{
    public class EnemyInRoomHandling : MonoBehaviour
    {
        public UnityEvent onAllEnemyDead;
        public int enemyCount;

        public void AddEnemy()
        {
            enemyCount++;
        }

        public void EnemyDied()
        {
            enemyCount--;
            if (enemyCount <= 0) onAllEnemyDead.Invoke();
        }
    }
}