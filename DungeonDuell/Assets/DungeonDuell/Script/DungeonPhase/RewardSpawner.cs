using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class RewardSpawner : BaseSingleSpawner
    {
        public void SpawnReward(){
            SpawnSingleObject(objectToSpawn);
        }
    }
}
