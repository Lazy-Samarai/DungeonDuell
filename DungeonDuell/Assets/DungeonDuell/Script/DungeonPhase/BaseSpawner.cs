using UnityEngine;

namespace dungeonduell
{
    public class BaseSpawner : MonoBehaviour // All Spawner just have same type
    {
        protected GameObject SpawnSingleObject(GameObject objectSpawn)
        {
            GameObject spawnedObject = null;
            if (objectSpawn != null) spawnedObject = Instantiate(objectSpawn, transform.position, Quaternion.identity, transform);
            return spawnedObject;
        }
    }
}