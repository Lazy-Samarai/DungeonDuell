using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class LockDown : MonoBehaviour
    {
        [SerializeField] List<GameObject> availableBarrier; 
       public void LockingDown(bool down)
       {
            foreach (GameObject g in availableBarrier)
            {
                g.SetActive(down);
            }
       }
    }
}
