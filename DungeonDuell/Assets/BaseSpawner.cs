using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class BaseSpwaner : MonoBehaviour
    {
        [SerializeField] GameObject objectToSpawn;
        // Start is called before the first frame update
        private void SpawnObject()
        {
            if(objectToSpawn != null)
            {
                Instantiate(objectToSpawn,transform.position,Quaternion.identity,transform);
            }
        }
    }
}
