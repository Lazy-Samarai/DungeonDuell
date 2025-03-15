using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using MoreMountains.TopDownEngine;
using System.Linq;

namespace dungeonduell
{
    public class HexgridController : MonoBehaviour
    {
        private Dictionary<Vector3Int, GameObject> tilePositions = new Dictionary<Vector3Int, GameObject>();
        private Vector3Int selectedTilePos;
        public GameObject cursor;
        private Tilemap tilemap;
        private PlayerInput playerInput;
        private TileClickHandler tileClickHandler;
        private CardToHand cardToHand;
        private bool isPlayer1Turn;

        void Start()
        {
            tilemap = FindObjectOfType<Tilemap>();
            tileClickHandler = FindObjectOfType<TileClickHandler>();
            playerInput = FindCorrectPlayerInput();

            if (playerInput != null)
            {
                playerInput.actions["Navigation"].performed += OnNavigate;
                playerInput.actions["Submit"].performed += OnSubmit;
                playerInput.actions["Back"].performed += OnBack;
            }
            else
            {
                Debug.LogError($"Kein passendes PlayerInput-Objekt für {gameObject.name} gefunden!");
            }

            InitializeTilePositions();
            if (cursor != null) cursor.SetActive(false);
            else Debug.LogError("Cursor-GameObject ist nicht zugewiesen!");
        }

        void OnDestroy()
        {
            if (playerInput != null)
            {
                playerInput.actions["Navigation"].performed -= OnNavigate;
                playerInput.actions["Submit"].performed -= OnSubmit;
                playerInput.actions["Back"].performed -= OnBack;
            }
        }

        private void InitializeTilePositions()
        {
            tilePositions.Clear();
            TileBase[] setAbleTiles = GetSetAbleTileBases();
            foreach (Vector3Int tilePos in GetSetAbleTiles(setAbleTiles))
            {
                tilePositions[tilePos] = null;
            }

            if (tilePositions.Count > 0)
            {
                selectedTilePos = new List<Vector3Int>(tilePositions.Keys)[0];
                UpdateCursor(selectedTilePos);
            }

            UpdateCursorVisibility();
        }

        /// <summary>
        /// Aktiviert das Hexgrid-Navigations-Interface (wie zuvor in HexgridControllerNavigation).
        /// </summary>
        public void ActivateNavigation()
        {
            if (tilePositions.Count > 0)
            {
                // Falls noch kein Tile ausgewählt ist, wähle das erste aus
                selectedTilePos = new List<Vector3Int>(tilePositions.Keys)[0];
                UpdateCursor(selectedTilePos);
                cursor.SetActive(true);
            }

            // Fokus auf das Hexgrid (bzw. den Cursor) setzen
            if (cursor != null)
            {
                EventSystem.current.SetSelectedGameObject(cursor);
            }
        }


        private void OnNavigate(InputAction.CallbackContext context)
        {
            if (!HasCardInCardHolder()) return;
            cursor.SetActive(true);

            Vector2 input = context.ReadValue<Vector2>();
            NavigateTiles(input);
        }

        private void NavigateTiles(Vector2 input)
        {
            Vector3Int currentPos = selectedTilePos;
            Vector3Int[] directions = {
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, -1, 0),
                new Vector3Int(1, -1, 0),
                new Vector3Int(-1, 1, 0)
            };

            Vector3Int targetPos = currentPos;
            if (input.y > 0) targetPos += directions[2];
            if (input.y < 0) targetPos += directions[3];
            if (input.x > 0) targetPos += directions[0];
            if (input.x < 0) targetPos += directions[1];

            if (tilePositions.ContainsKey(targetPos))
            {
                selectedTilePos = targetPos;
                UpdateCursor(selectedTilePos);
            }
        }

        private void UpdateCursor(Vector3Int position)
        {
            if (cursor != null)
            {
                cursor.transform.position = tilemap.GetCellCenterWorld(position);
                UpdateCursorVisibility();
            }
        }

        private void UpdateCursorVisibility()
        {
            if (cursor != null)
            {
                cursor.SetActive(HasCardInCardHolder());
            }
        }

        private void OnSubmit(InputAction.CallbackContext context)
        {
            if (!HasCardInCardHolder()) return;
            SpawnTileWithController(selectedTilePos);
            ResetNavigation();
        }

        private void OnBack(InputAction.CallbackContext context)
        {
            cursor.SetActive(false);
            ResetNavigation();
        }

        public void ResetNavigation()
        {
            cursor.SetActive(false);
        }

        private bool HasCardInCardHolder()
        {
            return cardToHand != null && cardToHand.HasCardOnHolder();
        }

        private TileBase[] GetSetAbleTileBases()
        {
            return tilemap.GetTilesBlock(tilemap.cellBounds);
        }

        private List<Vector3Int> GetSetAbleTiles(TileBase[] setAbleTiles)
        {
            List<Vector3Int> tilePositions = new List<Vector3Int>();
            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (setAbleTiles.Contains(tilemap.GetTile(pos)))
                {
                    tilePositions.Add(pos);
                }
            }
            return tilePositions;
        }

        private void SpawnTileWithController(Vector3Int tilePosition)
        {
            if (!HasCardInCardHolder() || tileClickHandler == null) return;

            Vector3 worldPosition = tilemap.GetCellCenterWorld(tilePosition);
            tileClickHandler.SpawnTile(worldPosition, tileClickHandler.currentCard, true, true, isPlayer1Turn ? 1 : 2);
        }

        private PlayerInput FindCorrectPlayerInput()
        {
            string neededPlayerID = IsPlayer1Controller() ? "Player1" : "Player2";
            foreach (var player in FindObjectsOfType<PlayerInput>())
            {
                var playerManager = player.GetComponent<InputSystemManagerEventsBased>();
                if (playerManager != null && playerManager.PlayerID == neededPlayerID)
                {
                    return player;
                }
            }
            return null;
        }

        public void AssignCardToHand(CardToHand newCardToHand)
        {
            cardToHand = newCardToHand;
        }

        private bool IsPlayer1Controller()
        {
            Transform parent = transform;
            while (parent != null)
            {
                if (parent.name == "CanvasPlayer_1") return true;
                if (parent.name == "CanvasPlayer_2") return false;
                parent = parent.parent;
            }
            return true;
        }
    }
}
