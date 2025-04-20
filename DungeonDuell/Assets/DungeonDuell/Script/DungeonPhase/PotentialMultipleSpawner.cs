using UnityEngine;

namespace dungeonduell
{
    public class PotentialMultipleSpawner : BaseMultipleSpawner
    {
        [SerializeField] [Range(0, 1)] private float chanceforMask;

        private void Start()
        {
            var chance = Random.Range(0f, 1f);
            if (chance > chanceforMask) SpawnRandomObject();
        }

        private void SpawnRandomObject()
        {
            SpawnSingleObject(objectsToSpawn[Random.Range(0, objectsToSpawn.Length)]);
        }
    }
}