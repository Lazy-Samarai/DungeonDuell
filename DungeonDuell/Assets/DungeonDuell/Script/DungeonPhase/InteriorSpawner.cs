using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
   
    public class InteriorSpawner : MonoBehaviour
    {
        [SerializeField] GameObject Interior;
        // Start is called before the first frame update
        void Start()
        {
            SpawnInterior();
        }
        public void SpawnInterior()
        {
            if(Interior != null)
            {
                Instantiate(Interior,transform.position,Quaternion.identity,transform);
            }
        }
    }
}
