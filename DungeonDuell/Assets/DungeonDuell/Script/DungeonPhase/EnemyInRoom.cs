using UnityEngine;

namespace dungeonduell
{
    public class EnemyInRoom : MonoBehaviour
    {
        private EnemyInRoomHandling _enemyInRoomHandling;

        // Start is called before the first frame update
        private void Start()
        {
            _enemyInRoomHandling = GetComponentInParent<EnemyInRoomHandling>();
            if (_enemyInRoomHandling != null) // if false Enemy Did not Spawn in a Enemy Room
                _enemyInRoomHandling.AddEnemy();
        }

        public void Death()
        {
            if (_enemyInRoomHandling != null) _enemyInRoomHandling.EnemyDied();
        }
    }
}