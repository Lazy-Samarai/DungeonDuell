using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class Debug_BreakLockDown : MonoBehaviour
    {
        [SerializeField] KeyCode breakLockDownKey = KeyCode.Delete;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(breakLockDownKey)){
                print("Break lock down");
                GetComponentInChildren<LockDown>().LockingDown(false);
            }
        
        }
    }
}
