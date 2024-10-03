using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
   
    public class RoomPortHandler : MonoBehaviour
    {
        public Tilemap tilemap;
        [SerializeField] List<GameObject> availablePort; // for some unclear stupid reason getting in Sart per get Child doesnt work
        List<Transform> usedPort;
    
        public void OpenPort(ConnectionDir dir)
        {
            GameObject port = GetPort(dir);

            foreach (Transform pos in GetRemovePostions(port))
            {
                Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(pos.position.x, pos.position.y, 0));

                tilemap.SetTile(cellPosition, null);
            }

            foreach(TileSetContainer container in GetSetPostions(port))
            {
                Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(container.transform.position.x, container.transform.position.y, 0));

                tilemap.SetTile(cellPosition, container.GetTile());
            }
        }
        private GameObject GetPort(ConnectionDir dir)
        {
            return availablePort[(int)dir];
        }

        private Transform[] GetRemovePostions(GameObject port)
        {
            return port.transform.GetChild(0).GetComponentsInChildren<Transform>().Skip(1).ToArray<Transform>();
        }
        private TileSetContainer[] GetSetPostions(GameObject port)
        {
            return port.transform.GetChild(1).GetComponentsInChildren<TileSetContainer>();
        }


    }
}
