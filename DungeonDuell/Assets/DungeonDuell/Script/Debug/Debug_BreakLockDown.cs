using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class Debug_BreakLockDown : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.L)){
                GetComponentInChildren<LockDown>().LockingDown(false);
            }
        
        }
    }
}
