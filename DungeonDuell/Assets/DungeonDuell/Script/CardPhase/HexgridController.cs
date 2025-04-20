using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    public class HexgridController : MonoBehaviour
    {
        private static readonly Vector2[] HexDirections =
        {
            new Vector2(1f, 0f).normalized,
            new Vector2(0.5f, 0.866f).normalized,
            new Vector2(-0.5f, 0.866f).normalized,
            new Vector2(-1f, 0f).normalized,
            new Vector2(-0.5f, -0.866f).normalized,
            new Vector2(0.5f, -0.866f).normalized
        };

        [Header("References")] [TagField] [SerializeField]
        private string TileMapTag;

        public Tilemap tilemap;
        public GameObject cursor;
        public TileClickHandler tileClickHandler;
        public TurnManager turnManager;
        public DisplayCard currentDisplayCard;
        private readonly float navigateCooldown = 0.15f;
        private readonly List<(Vector3Int, bool?)> setAbleTiles = new();
        private float lastNavigateTime;
        private PlayerInput playerInput;

        private Vector3Int selectedTilePos;

        private void Start()
        {
            if (turnManager == null)
                turnManager = FindFirstObjectByType<TurnManager>();

            playerInput = null;
            tilemap = FindObjectsByType<Tilemap>(FindObjectsSortMode.None)
                .FirstOrDefault(tm => tm.gameObject.tag == TileMapTag); // Becuase there is also the hovermap
        }

        public void ActivateNavigation()
        {
            setAbleTiles.Clear();
            foreach (var cellPos in tilemap.cellBounds.allPositionsWithin)
                if (tileClickHandler.IsSetablePosition(cellPos).Item1)
                    setAbleTiles.Add((cellPos, tileClickHandler.IsSetablePosition(cellPos).Item2));

            if (setAbleTiles.Count == 0)
            {
                Debug.LogWarning("Keine setzbaren Felder gefunden!");
                return;
            }

            Vector3Int? startPos = setAbleTiles.FirstOrDefault(tuple => tuple.Item2 == turnManager.isPlayer1Turn).Item1;

            selectedTilePos = startPos ?? setAbleTiles[0].Item1;

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

            var input = ctx.ReadValue<Vector2>();
            if (input.sqrMagnitude < 0.5f) return;

            var snappedInput = SnapToHexDirection(input.normalized);
            var bestTarget = selectedTilePos;
            var bestScore = -1f;

            const float dotThreshold = 0.65f;
            const float distanceThreshold = 0.1f;

            foreach (var tile in setAbleTiles)
            {
                if (tile.Item1 == selectedTilePos) continue;

                var dirToTile = new Vector2(tile.Item1.x - selectedTilePos.x, tile.Item1.y - selectedTilePos.y);
                var distance = dirToTile.magnitude;
                if (distance < distanceThreshold) continue;

                dirToTile.Normalize();
                var dot = Vector2.Dot(snappedInput, dirToTile);

                if (dot > dotThreshold)
                {
                    var score = dot * 1.5f - distance * 0.25f;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestTarget = tile.Item1;
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
            var bestDot = -1f;
            var bestDir = HexDirections[0];

            foreach (var dir in HexDirections)
            {
                var dot = Vector2.Dot(input, dir);
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

            var worldPos = tilemap.GetCellCenterWorld(selectedTilePos);

            var displayCard = tileClickHandler.displayCardUi;
            var card = tileClickHandler.currentCard;
            if (displayCard == null || card == null)
            {
                Debug.LogWarning("[HexgridController] Keine aktive Karte gefunden.");
                return;
            }

            var playerID = turnManager.isPlayer1Turn ? 1 : 2;
            var wasPlaced = tileClickHandler.SpawnTile(worldPos, card, true, true, playerID);

            if (!wasPlaced)
            {
                Debug.Log("Karte konnte nicht platziert werden – kein Reset.");
                return;
            }

            ResetNavigation();
            var cardToHand = GetCurrentCardToHand();
            if (cardToHand != null) cardToHand.ReactivateHandCards();
        }

        private void OnBack(InputAction.CallbackContext ctx)
        {
            var cardToHand = GetCurrentCardToHand();
            if (cardToHand.cardHolder.childCount > 0)
            {
                var child = cardToHand.cardHolder.GetChild(0);
                var dc = child.GetComponent<DisplayCard>();
                if (dc != null) cardToHand.OnCardClicked(dc);
            }

            ResetNavigation();
        }

        private CardToHand GetCurrentCardToHand()
        {
            return turnManager.isPlayer1Turn ? turnManager.HandPlayer1 : turnManager.HandPlayer2;
        }

        private PlayerInput GetCurrentPlayerInput()
        {
            var neededPlayerID = turnManager.isPlayer1Turn ? "Player1" : "Player2";

            foreach (var player in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
            {
                var manager = player.GetComponent<InputSystemManagerEventsBased>();
                if (manager != null && manager.PlayerID == neededPlayerID) return player;
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
            if (cardToHand != null) cardToHand.ReactivateHandCards();
        }
    }
}