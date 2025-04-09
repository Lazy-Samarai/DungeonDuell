using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using MoreMountains.TopDownEngine;
using UnityEngine.EventSystems;
using System.Linq;
using Cinemachine;

namespace dungeonduell
{
    public class HexgridController : MonoBehaviour
    {
        [Header("References")]
        [TagField] [SerializeField] private string TileMapTag;
        public Tilemap tilemap;
        public GameObject cursor;
        public TileClickHandler tileClickHandler;
        public TurnManager turnManager;
        public DisplayCard currentDisplayCard;

        private Vector3Int selectedTilePos;
        private List<Vector3Int> setAbleTiles = new List<Vector3Int>();
        private PlayerInput playerInput;
        private float lastNavigateTime = 0f;
        private float navigateCooldown = 0.15f;

        private static readonly Vector2[] HexDirections = new Vector2[]
        {
            new Vector2(1f, 0f).normalized,
            new Vector2(0.5f, 0.866f).normalized,
            new Vector2(-0.5f, 0.866f).normalized,
            new Vector2(-1f, 0f).normalized,
            new Vector2(-0.5f, -0.866f).normalized,
            new Vector2(0.5f, -0.866f).normalized
        };

        private void Start()
        {
            if (turnManager == null)
                turnManager = FindObjectOfType<TurnManager>();

            playerInput = null;
            tilemap = FindObjectsOfType<Tilemap>().FirstOrDefault(tm => tm.gameObject.tag == TileMapTag); // Becuase there is also the hovermap
            
            
        }

        public void ActivateNavigation()
        {
            setAbleTiles.Clear();
            foreach (var cellPos in tilemap.cellBounds.allPositionsWithin)
            {
                if (tileClickHandler.IsSetablePosition(cellPos))
                    setAbleTiles.Add(cellPos);
            }

            if (setAbleTiles.Count == 0)
            {
                Debug.LogWarning("Keine setzbaren Felder gefunden!");
                return;
            }

            selectedTilePos = setAbleTiles[0];
            if (cursor)
            {
                cursor.SetActive(true);
                cursor.transform.position = tilemap.GetCellCenterWorld(selectedTilePos);
            }

            DisableAllHandSelectables();
            if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null);

            playerInput = GetCurrentPlayerInput();
            if (playerInput != null)
            {
                playerInput.actions["Navigation"].performed += OnNavigate;
                playerInput.actions["Submit"].performed += OnSubmit;
                playerInput.actions["Back"].performed += OnBack;
            }
        }

        public void ResetNavigation()
        {
            if (cursor) cursor.SetActive(false);
            EnableAllHandSelectables();

            if (playerInput != null)
            {
                playerInput.actions["Navigation"].performed -= OnNavigate;
                playerInput.actions["Submit"].performed -= OnSubmit;
                playerInput.actions["Back"].performed -= OnBack;
                playerInput = null;
            }
        }

        private void OnNavigate(InputAction.CallbackContext ctx)
        {
            if (!cursor || !cursor.activeSelf || setAbleTiles.Count == 0) return;
            if (Time.time - lastNavigateTime < navigateCooldown) return;

            Vector2 input = ctx.ReadValue<Vector2>();
            if (input.sqrMagnitude < 0.5f) return;

            Vector2 snappedInput = SnapToHexDirection(input.normalized);
            Vector3Int bestTarget = selectedTilePos;
            float bestScore = -1f;
            
            const float dotThreshold = 0.65f;
            const float distanceThreshold = 0.1f;

            foreach (var tile in setAbleTiles)
            {
                if (tile == selectedTilePos) continue;

                Vector2 dirToTile = new Vector2(tile.x - selectedTilePos.x, tile.y - selectedTilePos.y);
                float distance = dirToTile.magnitude;
                if (distance < distanceThreshold) continue;

                dirToTile.Normalize();
                float dot = Vector2.Dot(snappedInput, dirToTile);

                if (dot > dotThreshold)
                {
                    float score = (dot * 1.5f) - (distance * 0.25f);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestTarget = tile;
                    }
                }
            }

            if (bestTarget != selectedTilePos)
            {
                selectedTilePos = bestTarget;
                cursor.transform.position = tilemap.GetCellCenterWorld(selectedTilePos);
                lastNavigateTime = Time.time;
            }
        }

        private Vector2 SnapToHexDirection(Vector2 input)
        {
            float bestDot = -1f;
            Vector2 bestDir = HexDirections[0];

            foreach (var dir in HexDirections)
            {
                float dot = Vector2.Dot(input, dir);
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestDir = dir;
                }
            }

            return bestDir;
        }

        private void OnSubmit(InputAction.CallbackContext ctx)
        {
            if (!cursor || !cursor.activeSelf || setAbleTiles.Count == 0)
                return;

            Vector3 worldPos = tilemap.GetCellCenterWorld(selectedTilePos);

            var displayCard = tileClickHandler.displayCardUi;
            var card = tileClickHandler.currentCard;
            if (displayCard == null || card == null)
            {
                Debug.LogWarning("[HexgridController] Keine aktive Karte gefunden.");
                return;
            }

            int playerID = turnManager.isPlayer1Turn ? 1 : 2;
            bool wasPlaced = tileClickHandler.SpawnTile(worldPos, card, true, true, playerID);

            if (!wasPlaced)
            {
                Debug.Log("Karte konnte nicht platziert werden – kein Reset.");
                return;
            }

            ResetNavigation();
            CardToHand cardToHand = GetCurrentCardToHand();
            if (cardToHand != null) cardToHand.ReactivateHandCards();
        }

        private void OnBack(InputAction.CallbackContext ctx)
        {
            CardToHand cardToHand = GetCurrentCardToHand();
            if (cardToHand.cardHolder.childCount > 0)
            {
                Transform child = cardToHand.cardHolder.GetChild(0);
                DisplayCard dc = child.GetComponent<DisplayCard>();
                if (dc != null)
                {
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

        private void DisableAllHandSelectables()
        {
            var cardToHand = GetCurrentCardToHand();
            if (cardToHand != null)
            {
                cardToHand.DisableHandCardsForNavigation();
                cardToHand.DeactivateHandCards();
            }
        }

        private void EnableAllHandSelectables()
        {
            var cardToHand = GetCurrentCardToHand();
            if (cardToHand != null)
            {
                cardToHand.ReactivateHandCards();
            }
        }
    }
}
