using UnityEngine;

namespace dungeonduell
{
    public class BaseSpawner : MonoBehaviour // All Spawner just have same type
    {
        protected void SpawnSingleObject(GameObject objectSpawn)
        {
            if (objectSpawn != null) Instantiate(objectSpawn, transform.position, Quaternion.identity, transform);
        }
    }
}