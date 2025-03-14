using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class BaseSpawner : MonoBehaviour
    {
        [SerializeField] GameObject objectToSpawn;
        // Start is called before the first frame update
        protected void SpawnObject()
        {
            if(objectToSpawn != null)
            {
                Instantiate(objectToSpawn,transform.position,Quaternion.identity,transform);
            }
        }
    }
}
