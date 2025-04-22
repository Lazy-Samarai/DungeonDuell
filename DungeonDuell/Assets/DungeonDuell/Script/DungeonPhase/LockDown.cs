using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class LockDown : MonoBehaviour
    {
        [SerializeField] private List<GameObject> availableBarrier;
        private bool _systemActive = true;

        public void LockingDown(bool down)
        {
            if (_systemActive)
                foreach (var g in availableBarrier)
                    if (g != null)
                        g.SetActive(down);
        }

        public void SetLockDownSystem(bool active)
        {
            _systemActive = active;
        }
    }
}