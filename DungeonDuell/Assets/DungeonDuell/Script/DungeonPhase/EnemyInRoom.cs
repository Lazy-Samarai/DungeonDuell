using UnityEngine;

namespace dungeonduell
{
    public class EnemyInRoom : MonoBehaviour
    {
        private EnemyInRoomHandling enemyInRoomHandling;

        // Start is called before the first frame update
        private void Start()
        {
            enemyInRoomHandling = GetComponentInParent<EnemyInRoomHandling>();
            if (enemyInRoomHandling != null) // if false Enemy Did not Spawn in a Enemy Room
                enemyInRoomHandling.AddEnemy();
        }

        public void Death()
        {
            if (enemyInRoomHandling != null) enemyInRoomHandling.EnemyDied();
        }
    }
}