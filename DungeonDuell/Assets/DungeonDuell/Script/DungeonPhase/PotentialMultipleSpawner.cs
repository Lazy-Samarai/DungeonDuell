using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace dungeonduell
{
    public class PotentialMultipleSpawner : BaseMultipleSpawner
    {
        [SerializeField]
        [Range(0,1)] float nothingChange;
        void Start()
        {
            float chance = Random.Range(0f, 1f);
            if (chance < nothingChange)
            {
                SpawnRandomObject();
            }
        }

        private void SpawnRandomObject()
        {
            SpawnSingleObject(objectsToSpawn[Random.Range(0, objectsToSpawn.Length)]);
        }
    }
}
