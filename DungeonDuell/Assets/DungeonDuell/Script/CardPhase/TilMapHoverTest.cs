using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem.UI;
using Cinemachine;

namespace dungeonduell
{

    public class TilMapHoverTest : MonoBehaviour, IObserver
    {
        const string visualAniNameRef = "Visible";
        public Camera cam;
        public TileBase hoverTile;

        public TileBase blockTile;

        public Tilemap tilemap;

        public Vector3Int currentCellPosition;

        public DoorIndicator indiactorDoorPrefab;
        public DoorIndicator runTimeIndicatorDoor;

        bool Currentlyvisble = false;

        [TagField] [SerializeField] private string cursorTag; // To Not Lose Refrence to Curi
        public GameObject controllerCursor;

        Animator animator;

        void Awake()
        {
            runTimeIndicatorDoor = Instantiate(indiactorDoorPrefab, transform.position, Quaternion.identity);
            runTimeIndicatorDoor.transform.parent = transform;
            runTimeIndicatorDoor.transform.gameObject.SetActive(false);
            animator = GetComponent<Animator>();
            tilemap = GetComponent<Tilemap>();

           
        }
            
        void Update()
        {
            if (Currentlyvisble)
            {
                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));
                HandleHover(mouseWorldPos);
                if (controllerCursor == null)
                {
                    controllerCursor = GameObject.FindGameObjectWithTag(cursorTag);
                }
                HandleHover(controllerCursor.transform.position);
            }

        }
        private void HandleHover(Vector3 mouseWorldPos)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(mouseWorldPos.x, mouseWorldPos.y, cam.transform.position.z));

            if (cellPosition != currentCellPosition)
            {
                if (tilemap.GetTile(cellPosition) != blockTile)
                {
                    ResetTileCheck();
                    tilemap.SetTile(cellPosition, hoverTile);
                    runTimeIndicatorDoor.transform.position = tilemap.CellToWorld(cellPosition);
                    runTimeIndicatorDoor.transform.gameObject.SetActive(Currentlyvisble ? true : false);
                }
                else
                {
                    ResetTileCheck();
                    runTimeIndicatorDoor.transform.gameObject.SetActive(false);
                }
                currentCellPosition = cellPosition;
            }
        }

        private void ResetTileCheck()
        {
            if (tilemap.GetTile(currentCellPosition) != blockTile)
            {
                tilemap.SetTile(currentCellPosition, null);
            }
        }

        public void SetHoverMapVisable(bool visual)
        {
            runTimeIndicatorDoor.transform.gameObject.SetActive(visual);
            animator.SetBool(visualAniNameRef, visual);
            Currentlyvisble = visual;
        }

        public void OnCardPlayed(Card card, bool p)
        {
            SetBlockOnTile(currentCellPosition);
            SetHoverMapVisable(false);
        }
        public void OnCardSelected(DisplayCard displayCard)
        {
            if (displayCard != null)
            {
                hoverTile = displayCard.card.Tile;
                SetHoverMapVisable(true);

                UpdateIndicator(displayCard.card.GetAllowedDirection());
            }
           
        }
        public void UpdateIndicator(bool[] allowedDoors)
        {
            runTimeIndicatorDoor.SetDoorIndiactor(allowedDoors);
        }
        public void OnPreSetCardSetOnTilemap(Card card, Vector3Int point)
        {
            SetBlockOnTile(point);
        }

        private void SetBlockOnTile(Vector3Int postion)
        {
            tilemap.SetTile(postion, blockTile);
        }
        void OnEnable()
        {
            cam = FindAnyObjectByType<Camera>();

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
            DDCodeEventHandler.CardRotating += UpdateIndicator;
        }

        public void UnsubscribeToAllEvents()
        {
            DDCodeEventHandler.CardPlayed -= OnCardPlayed;
            DDCodeEventHandler.PreSetCardSetOnTilemap -= OnPreSetCardSetOnTilemap;
            DDCodeEventHandler.CardSelected -= OnCardSelected;
            DDCodeEventHandler.CardRotating -= UpdateIndicator;
        }
    }
}
