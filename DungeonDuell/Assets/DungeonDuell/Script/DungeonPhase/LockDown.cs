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
            List<ConnectionDir> usedPort = GetComponent<RoomPortHandler>().usedPort;
            if (_systemActive)
                for (int g = 0; g < availableBarrier.Count; g++)
                    if (availableBarrier[g] != null)
                        if (usedPort.Contains((ConnectionDir)g))
                            availableBarrier[g].GetComponent<RoomBarrier>().LockdownBarrier(down);
        }

        public void SetLockDownSystem(bool active)
        {
            _systemActive = active;
        }
    }
}