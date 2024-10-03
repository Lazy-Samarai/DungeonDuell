using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
   
    public class InteriorSpawner : MonoBehaviour
    {
        [SerializeField] GameObject[] PossibleInterior;
        // Start is called before the first frame update
        public void SpawnInterior(RoomType roomtype)
        {
            if(roomtype != RoomType.Generic)
            {
                Instantiate(PossibleInterior[(int)roomtype - 1],transform.position,Quaternion.identity);
            }
        }
    }
}
