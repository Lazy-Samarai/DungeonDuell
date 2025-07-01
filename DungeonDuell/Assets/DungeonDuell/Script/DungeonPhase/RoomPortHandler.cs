using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class RoomPortHandler : MonoBehaviour
    {
        [SerializeField] private List<GameObject>
            availablePort; // for some unclear stupid reason getting in Sart per get Child doesnt work

        public List<ConnectionDir> usedPort;

        public void OpenPort(ConnectionDir dir)
        {
            usedPort.Add(dir);
            var port = GetPort(dir);

            port.SetActive(false);
        }

        private GameObject GetPort(ConnectionDir dir)
        {
            return availablePort[(int)dir];
        }
    }
}