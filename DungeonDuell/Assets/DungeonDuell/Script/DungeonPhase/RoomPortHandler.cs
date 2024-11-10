using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
   
    public class RoomPortHandler : MonoBehaviour
    {
        [SerializeField] List<GameObject> availablePort; // for some unclear stupid reason getting in Sart per get Child doesnt work
        List<Transform> usedPort;
    
        public void OpenPort(ConnectionDir dir)
        {
            GameObject port = GetPort(dir);

            port.SetActive(false);
        }
        private GameObject GetPort(ConnectionDir dir)
        {
            return availablePort[(int)dir];
        }

        /*
        private Transform[] GetRemovePostions(GameObject port)
        {
            return port.transform.GetChild(0).GetComponentsInChildren<Transform>().Skip(1).ToArray<Transform>();
        }
        private TileSetContainer[] GetSetPostions(GameObject port)
        {
            return port.transform.GetChild(1).GetComponentsInChildren<TileSetContainer>();
        }
        */


    }
}
