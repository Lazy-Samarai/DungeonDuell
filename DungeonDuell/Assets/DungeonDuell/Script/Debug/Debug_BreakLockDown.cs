using UnityEngine;

namespace dungeonduell
{
    public class DebugBreakLockDown : MonoBehaviour
    {
        [SerializeField] private KeyCode breakLockDownKey = KeyCode.Delete;

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKey(breakLockDownKey))
            {
                print("Break lock down");
                GetComponentInChildren<LockDown>().LockingDown(false);
            }
        }
    }
}