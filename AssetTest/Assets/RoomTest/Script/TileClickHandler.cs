using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace dungeonduell
{
    public class TileClickHandler : MonoBehaviour
    {
        public Camera cam;

        public Tilemap tilemap; 
        public TileBase setTile;
        public TileBase resetTile;

        public TileBase[] worldTiles;

        public ConnectionsCollector connectCollector;

        public GameObject StartTiles;


        Vector3Int[] aroundHexDiffVectorEVEN = { 
            new Vector3Int(-1, 1), // TopLeft
            new Vector3Int(0, 1), // TopRight

            new Vector3Int(-1, 0), // left
            new Vector3Int(1, 0), // right 

            new Vector3Int(-1, -1), // BottonLeft
            new Vector3Int(0, -1), // BottonRight 
        };


        Vector3Int[] aroundHexDiffVectorODD = {
            new Vector3Int(0, 1), // TopLeft
            new Vector3Int(1, 1), // TopRight

            new Vector3Int(-1, 0), // left
            new Vector3Int(1, 0), // right 

            new Vector3Int(0, -1), // BottonLeft
            new Vector3Int(1, -1), // BottonRight 
        };
        private void Start()
        {
            
            foreach (Transform transform in StartTiles.transform.GetChild(0).GetComponentInChildren<Transform>())
            {
                SpawnTile(transform.position, worldTiles[0]);
            }

            SpawnTile(StartTiles.transform.GetChild(1).GetChild(0).transform.position, worldTiles[1]);
            
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {

                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));
                SpawnTile(mouseWorldPos, setTile);

            }
        }

        private void SpawnTile(Vector3 mouseWorldPos, TileBase tileToSet)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(mouseWorldPos.x, mouseWorldPos.y, cam.transform.position.z));

            TileBase clickedTile = tilemap.GetTile(cellPosition);

            if (clickedTile == resetTile)
            {
                Debug.Log("Tile clicked at position: " + cellPosition);
                tilemap.SetTile(cellPosition, tileToSet);
                CreateRoom(cellPosition);


            }
            else
            {
                Debug.Log("Denied");
            }
        }

        private void CreateRoom(Vector3Int clickedTile)
        {
            Vector3Int[] aroundpos = new Vector3Int[6];

            var offsets = (clickedTile.y % 2 == 0) ? aroundHexDiffVectorEVEN : aroundHexDiffVectorODD;

            for (int i = 0; i < offsets.Length; i++)
            {
                aroundpos[i] = clickedTile + offsets[i];
            }
            

            int[] establishConnection = connectCollector.GetPossibleConnects(aroundpos);

            List<RoomConnection> Conncection = new List<RoomConnection>();

            for (int i = 0; i < establishConnection.Length; i++)
            {
                if(establishConnection[i] != -1)
                {
                    Conncection.Add(new RoomConnection(establishConnection[i], (ConnectionDir)i));
                }
               
            }

            connectCollector.AddRoom(clickedTile, Conncection);

        }
        public void ChangeSetTile(TileBase newTile)
        {
            setTile = newTile;
        }
    }
}
