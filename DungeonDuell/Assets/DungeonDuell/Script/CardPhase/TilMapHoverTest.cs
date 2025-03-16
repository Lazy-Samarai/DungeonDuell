using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    public class TilMapHoverTest : MonoBehaviour, IObserver
    {
        public Camera cam;
        public TileBase testTile;

        public TileBase blockTile;

        public Tilemap tilemap;

        public Vector3Int currentCellPosition;

        // Update is called once per frame
        void Update()
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));

            Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(mouseWorldPos.x, mouseWorldPos.y, cam.transform.position.z));

            if (cellPosition != currentCellPosition)
            {
                if (tilemap.GetTile(cellPosition) != blockTile)
                {
                    if (tilemap.GetTile(currentCellPosition) != blockTile)
                    {
                        tilemap.SetTile(currentCellPosition, null);
                    }
                    currentCellPosition = cellPosition;
                    tilemap.SetTile(currentCellPosition, testTile);
                }
                else
                {
                    tilemap.SetTile(currentCellPosition, null);
                }
            }
        }
        public void SetHoverMapVisable(bool visual)
        {
            tilemap.color = visual ? tilemap.color.WithAlpha(255) : tilemap.color.WithAlpha(0);
        }

        public void OnCardPlayed(Card card, bool p)
        {
            SetBlockOnTile(currentCellPosition);
            SetHoverMapVisable(false);
        }
        public void OnCardSelected(DisplayCard displayCard){
            SetHoverMapVisable(true);
        }
        public void OnPreSetCardSetOnTilemap(Card card,Vector3Int point)
        {
            SetBlockOnTile(point);
        }

        private void SetBlockOnTile(Vector3Int postion)
        {
            tilemap.SetTile(postion, blockTile);
        }
        void OnEnable()
        {
            SubscribeToEvents();
        }
        void OnDisable()
        {
            UnsubscribeToAllEvents();
        }
        public void SubscribeToEvents()
        {
            DDCodeEventHandler.CardPlayed += OnCardPlayed;
            DDCodeEventHandler.PreSetCardSetOnTilemap += OnPreSetCardSetOnTilemap;
            DDCodeEventHandler.CardSelected += OnCardSelected;
        }

        public void UnsubscribeToAllEvents()
        {
            DDCodeEventHandler.CardPlayed -= OnCardPlayed;
            DDCodeEventHandler.PreSetCardSetOnTilemap -= OnPreSetCardSetOnTilemap;
            DDCodeEventHandler.CardSelected += OnCardSelected;
        }
    }
}
