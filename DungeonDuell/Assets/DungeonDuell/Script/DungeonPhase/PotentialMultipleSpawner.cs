using UnityEngine;

namespace dungeonduell
{
    public class PotentialMultipleSpawner : BaseMultipleSpawner
    {
        public void SpawnRandomObject()
        {
           GameObject obj = SpawnSingleObject(objectsToSpawn[Random.Range(0, objectsToSpawn.Length)]);
           obj.GetComponent<ActiveColliderOnTime>().Interrupt();

        }
    }
}