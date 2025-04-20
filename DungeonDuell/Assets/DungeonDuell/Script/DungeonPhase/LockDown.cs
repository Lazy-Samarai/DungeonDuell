using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class LockDown : MonoBehaviour
    {
        [SerializeField] private List<GameObject> availableBarrier;
        private bool SystemActive = true;

        public void LockingDown(bool down)
        {
            if (SystemActive)
                foreach (var g in availableBarrier)
                    if (g != null)
                        g.SetActive(down);
        }

        public void SetLockDownSystem(bool active)
        {
            SystemActive = active;
        }
    }
}