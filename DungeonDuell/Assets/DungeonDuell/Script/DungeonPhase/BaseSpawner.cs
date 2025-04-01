using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class BaseSpawner : MonoBehaviour // Just so all of this OnTime Spawner have same ground Time
    {
        protected void SpawnSingleObject(GameObject objectSpawn)
        {
            if(objectSpawn != null)
            {
                Instantiate(objectSpawn,transform.position,Quaternion.identity,transform);
            }
        }
    }
}
