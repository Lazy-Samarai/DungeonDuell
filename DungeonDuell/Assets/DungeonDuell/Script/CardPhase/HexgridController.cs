using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    public class HexgridController : MonoBehaviour
    {
        public Camera cam;

        private static readonly Vector2[] HexDirections =
        {
            new Vector2(1f, 0f).normalized,
            new Vector2(0.5f, 0.866f).normalized,
            new Vector2(-0.5f, 0.866f).normalized,
            new Vector2(-1f, 0f).normalized,
            new Vector2(-0.5f, -0.866f).normalized,
            new Vector2(0.5f, -0.866f).normalized
        };

        [FormerlySerializedAs("TileMapTag")] [Header("References")] [TagField] [SerializeField]
        private string tileMapTag;

        public Tilemap tilemap;
        public GameObject cursor;
        public TileClickHandler tileClickHandler;
        public TurnManager turnManager;
        public DisplayCard currentDisplayCard;
        private readonly float _navigateCooldown = 0.15f;
        private readonly List<(Vector3Int, bool?)> _setAbleTiles = new();
        private float _lastNavigateTime;
        private PlayerInput _playerInput;

        private Vector3Int _selectedTilePos;

        private void Start()
        {
            if (turnManager == null)
                turnManager = FindFirstObjectByType<TurnManager>();

            _playerInput = null;
            tilemap = FindObjectsByType<Tilemap>(FindObjectsSortMode.None)
                .FirstOrDefault(tm => tm.gameObject.CompareTag(tileMapTag)); // Becuase there is also the hovermap
        }

        public void ActivateNavigation()
        {
            _setAbleTiles.Clear();
            foreach (var cellPos in tilemap.cellBounds.allPositionsWithin)
                if (tileClickHandler.IsSetablePosition(cellPos).Item1)
                    _setAbleTiles.Add((cellPos, tileClickHandler.IsSetablePosition(cellPos).Item2));

            if (_setAbleTiles.Count == 0)
            {
                Debug.LogWarning("Keine setzbaren Felder gefunden!");
                return;
            }

            Vector3Int startPos =
                _setAbleTiles.FirstOrDefault(tuple => tuple.Item2 == turnManager.isPlayer1Turn).Item1;

            _selectedTilePos = startPos;

            if (cursor)
            {
                cursor.SetActive(true);
                cursor.transform.position = tilemap.GetCellCenterWorld(_selectedTilePos);
            }

            DisableAllHandSelectables();
            if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null);

            _playerInput = GetCurrentPlayerInput();
            if (_playerInput != null)
            {
                _playerInput.actions["Navigation"].performed += OnNavigate;
                _playerInput.actions["Submit"].performed += OnSubmit;
                _playerInput.actions["Back"].performed += OnBack;
            }
        }

        public void ResetNavigation()
        {
            if (cursor) cursor.SetActive(false);
            EnableAllHandSelectables();

            if (_playerInput != null)
            {
                _playerInput.actions["Navigation"].performed -= OnNavigate;
                _playerInput.actions["Submit"].performed -= OnSubmit;
                _playerInput.actions["Back"].performed -= OnBack;
                _playerInput = null;
            }
        }

        private void OnNavigate(InputAction.CallbackContext ctx)
        {
            if (!cursor || !cursor.activeSelf || _setAbleTiles.Count == 0) return;
            if (Time.time - _lastNavigateTime < _navigateCooldown) return;

            var input = ctx.ReadValue<Vector2>();
            if (input.sqrMagnitude < 0.5f) return;
            var snappedInput = SnapToHexDirection(input.normalized);
            var bestTarget = _selectedTilePos;
            var bestScore = -1f;

            const float dotThreshold = 0.65f;
            const float distanceThreshold = 0.1f;

            foreach (var tile in _setAbleTiles)
            {
                if (tile.Item1 == _selectedTilePos) continue;

                if (ctx.control.device is Mouse)
                {
                    Vector3 mouseWorldPos =
                        cam.ScreenToWorldPoint(new Vector3(input.x, input.y, -cam.transform.position.z));
                    Vector3Int cellPosition =
                        tilemap.WorldToCell(new Vector3(mouseWorldPos.x, mouseWorldPos.y, cam.transform.position.z));

                    if (tile.Item1 == cellPosition)
                    {
                        bestTarget = cellPosition;
                    }
                }
                else
                {
                    Vector2 dirToTile = new Vector2(tile.Item1.x - _selectedTilePos.x,
                        tile.Item1.y - _selectedTilePos.y);


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
            }

            if (bestTarget != _selectedTilePos)
            {
                _selectedTilePos = bestTarget;
                cursor.transform.position = tilemap.GetCellCenterWorld(_selectedTilePos);
                _lastNavigateTime = Time.time;
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
            if (!cursor || !cursor.activeSelf || _setAbleTiles.Count == 0)
                return;

            var worldPos = tilemap.GetCellCenterWorld(_selectedTilePos);

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
            return turnManager.isPlayer1Turn ? turnManager.handPlayer1 : turnManager.handPlayer2;
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