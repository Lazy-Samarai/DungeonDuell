using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class LockDown : MonoBehaviour
    {
        [SerializeField] List<GameObject> availableBarrier;
        bool SystemActive = true;
        public void LockingDown(bool down)
        {
            if (SystemActive)
            {
                foreach (GameObject g in availableBarrier)
                {
                    if(g != null){
                        g.SetActive(down);
                    }
                   
                }
            }
        }
        public void SetLockDownSystem(bool active){
            SystemActive = active;
        }

    }
}
