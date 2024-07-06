using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    public class TileClickHandler : MonoBehaviour
    {
        public Camera cam;

        public Tilemap tilemap; 
        public TileBase setTile;

        public TileBase resetTile;

        void Update()
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                
                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));



                Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(mouseWorldPos.x, mouseWorldPos.y, cam.transform.position.z)); // new Vector3(mouseWorldPos.x, mouseWorldPos.y, tilemap.transform.postion.z)

                print("cellPosition:");
                print(cellPosition);

                TileBase clickedTile = tilemap.GetTile(cellPosition);

                if (clickedTile == resetTile) 
                {
                    Debug.Log("Tile clicked at position: " + cellPosition);
                    tilemap.SetTile(cellPosition, setTile); 
                }
                else
                {
                    Debug.Log("Denied");
                }
                

            }
        }
    }
}
