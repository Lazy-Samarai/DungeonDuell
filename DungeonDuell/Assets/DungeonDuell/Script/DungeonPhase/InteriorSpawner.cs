using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
   
    public class InteriorSpawner : BaseSingleSpawner
    {
      
        void Start()
        {
            SpawnSingleObject(objectToSpawn);
        }
    }
}
