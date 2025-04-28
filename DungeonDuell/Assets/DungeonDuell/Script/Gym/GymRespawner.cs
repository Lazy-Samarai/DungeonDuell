using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace dungeonduell
{
    public class GymRespawner : BaseSingleSpawner
    {
        //public GameObject ObjectToDestroy;
        public void GymEnemyRespawn()
        {
            SpawnSingleObject(objectToSpawn);
            //Destroy(ObjectToDestroy);
        }
    }
}
