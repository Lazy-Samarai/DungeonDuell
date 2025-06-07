using UnityEngine;

namespace dungeonduell
{
    public class GymRespawner : BaseSingleSpawner
    {
        [Tooltip("Zeit in Sekunden, bis ein Gegner gespawnt wird")]
        [SerializeField] private float respawnDelay = 3f;

        public void GymEnemyRespawn()
        {
            Invoke(nameof(Spawn), respawnDelay);
        }

        private void Spawn()
        {
            SpawnSingleObject(objectToSpawn);
        }
    }
}
