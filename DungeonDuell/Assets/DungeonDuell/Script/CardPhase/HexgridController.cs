using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using MoreMountains.TopDownEngine;

namespace dungeonduell
{
    public class HexgridController : MonoBehaviour
    {
        [Header("Player Settings")]
        public bool isPlayerOne;

        [Header("References")]
        public Tilemap tilemap;
        public GameObject cursor;
        public TileClickHandler tileClickHandler;
        public TurnManager turnManager;

        public DisplayCard currentDisplayCard;

        private Vector3Int selectedTilePos;

        private static readonly Vector3Int[] OffsetsEven =
        {
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(1, -1, 0)
        };

        private static readonly Vector3Int[] OffsetsOdd =
        {
            new Vector3Int(-1, 1, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, -1, 0),
            new Vector3Int(0, -1, 0)
        };

        private void Start()
        {
            if (turnManager == null)
            {
                turnManager = FindObjectOfType<TurnManager>();
            }

            PlayerInput playerInput = GetCurrentPlayerInput();

            if (cursor) cursor.SetActive(false);

            if (playerInput != null)
            {
                playerInput.actions["Navigation"].performed += OnNavigate;
                playerInput.actions["Submit"].performed += OnSubmit;
                playerInput.actions["Back"].performed += OnBack;
            }
        }

        private void OnDestroy()
        {
            PlayerInput playerInput = GetCurrentPlayerInput();

            if (playerInput != null)
            {
                playerInput.actions["Navigation"].performed -= OnNavigate;
                playerInput.actions["Submit"].performed -= OnSubmit;
                playerInput.actions["Back"].performed -= OnBack;
            }
        }

        public void ActivateNavigation()
        {
            BoundsInt bounds = tilemap.cellBounds;
            bool foundValid = false;

            foreach (var cellPos in bounds.allPositionsWithin)
            {
                if (tileClickHandler.IsSetablePosition(cellPos))
                {
                    selectedTilePos = cellPos;
                    foundValid = true;
                    break;
                }
            }

            if (!foundValid)
            {
                Debug.LogWarning("Kein setzbares Feld in der Tilemap gefunden!");
                return;
            }

            if (cursor)
            {
                cursor.SetActive(true);
                cursor.transform.position = tilemap.GetCellCenterWorld(selectedTilePos);
            }
        }

        public void ResetNavigation()
        {
            if (cursor) cursor.SetActive(false);
        }

        private void OnNavigate(InputAction.CallbackContext ctx)
        {
            if (!cursor || !cursor.activeSelf) return;

            Vector2 input = ctx.ReadValue<Vector2>();
            if (input.sqrMagnitude < 0.3f) return;

            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            int sector = Mathf.RoundToInt(angle / 60f) % 6;
            Vector3Int[] offsets = (selectedTilePos.y % 2 == 0) ? OffsetsEven : OffsetsOdd;
            Vector3Int neighborPos = selectedTilePos + offsets[sector];

            if (tileClickHandler.IsSetablePosition(neighborPos))
            {
                selectedTilePos = neighborPos;
                cursor.transform.position = tilemap.GetCellCenterWorld(selectedTilePos);
            }
        }

        private void OnSubmit(InputAction.CallbackContext ctx)
        {
            if (!cursor || !cursor.activeSelf) return;

            Vector3 worldPos = tilemap.GetCellCenterWorld(selectedTilePos);

            tileClickHandler.SpawnTile(
                worldPos,
                tileClickHandler.currentCard,
                true,
                true,
                turnManager.isPlayer1Turn ? 1 : 2
            );

            ResetNavigation();
        }

        private void OnBack(InputAction.CallbackContext ctx)
        {
            Debug.Log("OnBack called!");

            CardToHand cardToHand = GetCurrentCardToHand();
            if (cardToHand.cardHolder.childCount > 0)
            {
                Transform child = cardToHand.cardHolder.GetChild(0);
                DisplayCard dc = child.GetComponent<DisplayCard>();
                Debug.Log(child.name);
                if (dc != null)
                {
                    Debug.Log("OnBack works!");
                    cardToHand.OnCardClicked(dc);
                }
            }

            ResetNavigation();
        }

        private CardToHand GetCurrentCardToHand()
        {
            return turnManager.isPlayer1Turn ? turnManager.HandPlayer1 : turnManager.HandPlayer2;
        }

        private PlayerInput GetCurrentPlayerInput()
        {
            string neededPlayerID = turnManager.isPlayer1Turn ? "Player1" : "Player2";

            foreach (var player in FindObjectsOfType<PlayerInput>())
            {
                var manager = player.GetComponent<InputSystemManagerEventsBased>();
                if (manager != null && manager.PlayerID == neededPlayerID)
                {
                    return player;
                }
            }

            return null;
        }
    }
}
