using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using MoreMountains.TopDownEngine;

namespace dungeonduell
{
    public class HexgridControllerNavigation : MonoBehaviour
    {
        private Dictionary<Vector3Int, GameObject> tilePositions = new Dictionary<Vector3Int, GameObject>();
        private Vector3Int selectedTilePos;
        public GameObject cursor; 
        public Tilemap tilemap;
        private PlayerInput playerInput;
        public TileClickHandler tileClickHandler;
        private CardToHand cardToHand; 

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

            if (cursor != null)
            {
                cursor.SetActive(false);
            }
            else
            {
                Debug.LogError("Cursor-GameObject ist nicht zugewiesen!");
            }
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
            if (tileClickHandler != null)
            {
                foreach (Vector3Int tilePos in tileClickHandler.GetSetAbleTiles())
                {
                    tilePositions[tilePos] = null;
                }
            }

            if (tilePositions.Count > 0)
            {
                selectedTilePos = new List<Vector3Int>(tilePositions.Keys)[0];
                UpdateCursor(selectedTilePos);
            }

            UpdateCursorVisibility();
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
            Vector3Int[] directions = new Vector3Int[]
            {
                new Vector3Int(1, 0, 0),  // Rechts
                new Vector3Int(-1, 0, 0), // Links
                new Vector3Int(0, 1, 0),  // Oben
                new Vector3Int(0, -1, 0), // Unten
                new Vector3Int(1, -1, 0), // Diagonal oben-rechts
                new Vector3Int(-1, 1, 0)  // Diagonal oben-links
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

        public void ActivateNavigation()
        {
            if (tilePositions.Count > 0)
            {
                selectedTilePos = new List<Vector3Int>(tilePositions.Keys)[0];
                UpdateCursor(selectedTilePos);
                cursor.SetActive(true);
            }

            EventSystem.current.SetSelectedGameObject(cursor); // Fokus auf das Hexgrid setzen
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

            if (tileClickHandler != null)
            {
                Debug.Log($"Tile bestätigt: {selectedTilePos}");
                tileClickHandler.SpawnTileWithController(selectedTilePos);
                ResetNavigation();
            }
        }

        private void OnBack(InputAction.CallbackContext context)
        {
            Debug.Log("Zurück zur Hand!");
            cursor.SetActive(false);
            ResetNavigation();
        }

        public void ResetNavigation()
        {
            if (tilePositions.Count > 0)
            {
                selectedTilePos = new List<Vector3Int>(tilePositions.Keys)[0];
                UpdateCursor(selectedTilePos);
            }

            UpdateCursorVisibility();
        }

        private bool HasCardInCardHolder()
        {
            return cardToHand != null && cardToHand.HasCardOnHolder();
        }

        /// <summary>
        /// Holt das richtige PlayerInput-Objekt basierend auf der PlayerID (Player1 oder Player2)
        /// </summary>
        private PlayerInput FindCorrectPlayerInput()
        {
            string neededPlayerID = IsPlayer1Controller() ? "Player1" : "Player2";

            foreach (var player in FindObjectsOfType<PlayerInput>())
            {
                var playerManager = player.GetComponent<InputSystemManagerEventsBased>();
                if (playerManager != null && playerManager.PlayerID == neededPlayerID)
                {
                    return player; // Richtige PlayerInput-Instanz gefunden
                }
            }
            return null;
        }

        /// <summary>
        /// Wird vom TurnManager aufgerufen, um das richtige CardToHand-Objekt zu setzen.
        /// </summary>
        public void AssignCardToHand(CardToHand newCardToHand)
        {
            cardToHand = newCardToHand;
        }


        /// <summary>
        /// Prüft, ob dieser Controller zu Player1 oder Player2 gehört.
        /// </summary>
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
