using UnityEngine;

namespace dungeonduell
{
    public class SecondaryInterior : BaseMultipleSpawner
    {
        public void SpawnExtraInterior(SecondaryRoomType secondaryRoomType)
        {
            if (secondaryRoomType != SecondaryRoomType.Generic)
            {
                SpawnSingleObject(objectsToSpawn[(int)secondaryRoomType - 1]);
            }
        }
    }
}