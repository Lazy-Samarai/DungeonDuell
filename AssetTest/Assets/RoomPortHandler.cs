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
            foreach (Transform pos in GetRemovePostions(dir))
            {
                Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(pos.position.x, pos.position.y, 0));

                tilemap.SetTile(cellPosition, null);
            }
        }

        private Transform[] GetRemovePostions(ConnectionDir dir)
        {
            return availablePort[(int)dir].GetComponentsInChildren<Transform>().Skip(1).ToArray<Transform>();
        }


    }
}
