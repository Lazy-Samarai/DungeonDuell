using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class DoorIndicator : MonoBehaviour
    {
        [SerializeField] bool inverted = false;
        [SerializeField] Transform dirAnker;
        public void SetDoorIndiactor(bool[] allowedDoors)
        {
            for(int i = 0;i < allowedDoors.Length;i++)
            {
                dirAnker.transform.GetChild(i).transform.gameObject.SetActive(inverted ? !allowedDoors[i] : allowedDoors[i]);
            }
        }

        public void OverExtend(bool[] allowedDoors)
        {
            for(int i = 0;i < allowedDoors.Length;i++)
            {
                if(allowedDoors[i]){
                    
                var scale = dirAnker.GetChild(i).transform.localScale;
                dirAnker.transform.GetChild(i).transform.localScale = new Vector3(scale.x * 2, scale.y, scale.z);
                }
              
            }
          
        }
    }
    
}
