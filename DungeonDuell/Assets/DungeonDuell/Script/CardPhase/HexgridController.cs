using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using MoreMountains.TopDownEngine;
using UnityEngine.EventSystems;
using System.Linq;

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
        private List<Vector3Int> setAbleTiles = new List<Vector3Int>();
        private PlayerInput playerInput;

        private float lastNavigateTime = 0f;
        private float navigateCooldown = 0.15f; // 150ms Pause zwischen Bewegungen

        private static readonly Vector2[] HexDirections = new Vector2[]
        {
            new Vector2(1f, 0f).normalized,           // Rechts
            new Vector2(0.5f, 0.866f).normalized,     // Oben rechts
            new Vector2(-0.5f, 0.866f).normalized,    // Oben links
            new Vector2(-1f, 0f).normalized,          // Links
            new Vector2(-0.5f, -0.866f).normalized,   // Unten links
            new Vector2(0.5f, -0.866f).normalized     // Unten rechts
        };

        private void Start()
        {
            if (turnManager == null)
            {
                turnManager = FindObjectOfType<TurnManager>();
            }

            playerInput = GetCurrentPlayerInput();

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
            if (playerInput != null)
            {
                playerInput.actions["Navigation"].performed -= OnNavigate;
                playerInput.actions["Submit"].performed -= OnSubmit;
                playerInput.actions["Back"].performed -= OnBack;
            }
        }

        public void ActivateNavigation()
        {
            setAbleTiles.Clear();
            BoundsInt bounds = tilemap.cellBounds;

            foreach (var cellPos in bounds.allPositionsWithin)
            {
                if (tileClickHandler.IsSetablePosition(cellPos))
                {
                    setAbleTiles.Add(cellPos);
                }
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

            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        public void ResetNavigation()
        {
            if (cursor) cursor.SetActive(false);
            EnableAllHandSelectables();
        }

        private void OnNavigate(InputAction.CallbackContext ctx)
        {
            if (!cursor || !cursor.activeSelf || setAbleTiles.Count == 0) return;
            if (Time.time - lastNavigateTime < navigateCooldown) return;

            Vector2 input = ctx.ReadValue<Vector2>();
            if (input.sqrMagnitude < 0.5f) return; // höhere Deadzone

            Vector2 snappedInput = SnapToHexDirection(input.normalized);

            Vector3Int bestTarget = selectedTilePos;
            float bestScore = -1f;

            foreach (var tile in setAbleTiles)
            {
                if (tile == selectedTilePos) continue;

                Vector2 dirToTile = new Vector2(tile.x - selectedTilePos.x, tile.y - selectedTilePos.y);
                float distance = dirToTile.magnitude;
                if (distance < 0.1f) continue;

                dirToTile.Normalize();
                float dot = Vector2.Dot(snappedInput, dirToTile);

                if (dot > 0.65f)
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

            // Nur wenn Karte platziert wurde:
            ResetNavigation();

            CardToHand cardToHand = GetCurrentCardToHand();
            if (cardToHand != null)
            {
                cardToHand.ReactivateHandCards();
            }
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
