using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    public class TileClickHandler : MonoBehaviour, IObserver
    {
        public Camera cam;

        [FormerlySerializedAs("TileMapTag")] [TagField] [SerializeField]
        private string tileMapTag;

        public Card currentCard;
        public bool[] currentDoorDir = { true, true, true, true, true, true };
        public DisplayCard displayCardUi;
        public GameObject indiactorDoor;
        public Transform indiactorDoorAnker;
        public TileBase resetTile;
        public TileBase[] setAbleTiles;
        public TileBase[] shadowSetAbleTiles;
        [FormerlySerializedAs("CardShelled")] public List<ShellCard> cardShelled;
        public ConnectionsCollector connectCollector;
        public bool isPlayer1Turn = true;

        private readonly Vector3Int[] _aroundHexDiffVectorEven =
        {
            new(-1, 1), // TopLeft
            new(0, 1), // TopRight

            new(-1, 0), // left
            new(1, 0), // right 

            new(-1, -1), // BottonLeft
            new(0, -1) // BottonRight 
        };

        private readonly Vector3Int[] _aroundHexDiffVectorOdd =
        {
            new(0, 1), // TopLeft
            new(1, 1), // TopRight

            new(-1, 0), // left
            new(1, 0), // right 

            new(0, -1), // BottonLeft
            new(1, -1) // BottonRight 
        };

        private HexgridController _hexgridController;
        private Tilemap _tilemap;
        private TurnManager _turnManager;


        private void Start()
        {
            connectCollector = FindFirstObjectByType<ConnectionsCollector>();
            _tilemap = FindObjectsByType<Tilemap>(FindObjectsSortMode.None)
                .FirstOrDefault(tm => tm.gameObject.tag == tileMapTag); // Becuase there is also the hovermap
            _turnManager = FindFirstObjectByType<TurnManager>();
            _hexgridController = FindFirstObjectByType<HexgridController>();
        }

        private void Update()
        {
            if (currentCard != null)
                if (Input.GetMouseButtonDown(0))
                {
                    var mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                        Input.mousePosition.y, -cam.transform.position.z));
                    SpawnTile(mouseWorldPos, currentCard, true, true, isPlayer1Turn ? 1 : 2);
                }
        }


        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToAllEvents();
        }

        public void SubscribeToEvents()
        {
            DdCodeEventHandler.CardSelected += ChangeCard;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.CardSelected -= ChangeCard;
        }

        public (bool, bool?) IsSetablePosition(Vector3Int cellPosition)
        {
            var tile = _tilemap.GetTile(cellPosition);
            if (tile == null) return (false, null); // kein Tile = ungültig

            // Gültig, wenn NICHT das resetTile und in setAbleTiles enthalten
            if (tile != resetTile && setAbleTiles.Contains(tile) |
                cardShelled.Any(card => card.inPlayerRangeTile.Contains(tile)))
            {
                if ((setAbleTiles[0] == tile) | cardShelled.Any(card => card.inPlayerRangeTile[0] == tile))
                    return (true, true);

                if ((setAbleTiles[1] == tile) | cardShelled.Any(card => card.inPlayerRangeTile[1] == tile))
                    return (true, false);

                return (true, null);
            }

            return (false, null);
        }


        public bool SpawnTile(Vector3 mouseWorldPos, Card card, bool playerMove, bool spawnSourroundSetables, int owner)
        {
            var cellPosition =
                _tilemap.WorldToCell(new Vector3(mouseWorldPos.x, mouseWorldPos.y, cam.transform.position.z));
            var clickedTile = _tilemap.GetTile(cellPosition);

            // Sonderfall: ShellCard
            if (card is ShellCard)
            {
                if (card.tile is AnimatedTile) _tilemap.SetTile(cellPosition, null);

                _tilemap.SetTile(cellPosition, card.tile);

                FinalizePlacement();
                return true;
            }

            if (clickedTile != resetTile || !playerMove)
            {
                // ShellCard-Verarbeitung
                var shelledTileCard = cardShelled.FirstOrDefault(x => x.inPlayerRangeTile.Contains(clickedTile));
                if (shelledTileCard != null)
                {
                    DdCodeEventHandler.Trigger_CardToShelled(card, isPlayer1Turn);


                    var index = Array.FindIndex(shelledTileCard.inPlayerRangeTile, tile => clickedTile == tile);
                    clickedTile = setAbleTiles[index];
                    shelledTileCard.startDoorConcellation = card.startDoorConcellation;

                    card = (Card)shelledTileCard.Clone();
                    card.tile = shelledTileCard.completeTile;
                }

                // Nur fortfahren, wenn gültig
                if ((setAbleTiles.Contains(clickedTile) && currentCard != null) || !playerMove)
                {
                    var wasHandled = CardUsingHandling(card, playerMove, spawnSourroundSetables, cellPosition,
                        clickedTile, owner);

                    if (wasHandled)
                    {
                        FinalizePlacement();
                        return true;
                    }

                    Debug.Log("[TileClickHandler] Platzierung durch CardUsingHandling abgelehnt");
                    return false;
                }

                Debug.Log("[TileClickHandler] Denied_SetOrNoCard");
                return false;
            }

            Debug.Log("[TileClickHandler] OutOfReachTile");
            return false;
        }


        private bool CardUsingHandling(Card card, bool playerMove, bool spawnSourroundSetables, Vector3Int cellPosition,
            TileBase clickedTile, int owner)
        {
            var overriteCurrentDoorDir = new[] { false, false, false, false, false, false };
            var connectionForcing = false;

            if (clickedTile == setAbleTiles[setAbleTiles.Length - 1]) // Hited Contested
            {
                DdCodeEventHandler.Trigger_DungeonConnected();
                connectionForcing = true;
                var offset = cellPosition.y % 2 == 0 ? _aroundHexDiffVectorEven : _aroundHexDiffVectorOdd;
                for (var i = 0; i < offset.Length; i++)
                    if (connectCollector.GetFullRoomList().Any(entry => entry.Item1 == cellPosition + offset[i]))
                    {
                        currentDoorDir[i] = true;
                        overriteCurrentDoorDir[i] = true;
                    }
            }

            var sourroundCorr = GetSouroundCorr(cellPosition, currentDoorDir);

            if (CheckConnectAblity(sourroundCorr) || !playerMove)
            {
                // --- HIER bleibt alles wie im Original ---
                _tilemap.SetTile(cellPosition, card.tile);

                if (spawnSourroundSetables)
                {
                    foreach (var sourrendTilePos in GetSouroundCorr(cellPosition,
                                 new[] { true, true, true, true, true, true }))
                    {
                        var souroundTile = _tilemap.GetTile(sourrendTilePos.Item1);

                        if (setAbleTiles.Contains(souroundTile))
                        {
                            if (clickedTile != souroundTile)
                                _tilemap.SetTile(sourrendTilePos.Item1, setAbleTiles[setAbleTiles.Length - 1]);
                        }
                        else if (shadowSetAbleTiles.Contains(souroundTile))
                        {
                            var i = Array.FindIndex(setAbleTiles, entity => entity == clickedTile);
                            if (i == setAbleTiles.Length - 1)
                                _tilemap.SetTile(sourrendTilePos.Item1, setAbleTiles[setAbleTiles.Length - 1]);
                            else if (souroundTile != shadowSetAbleTiles[i])
                                _tilemap.SetTile(sourrendTilePos.Item1, setAbleTiles[setAbleTiles.Length - 1]);
                        }

                        if (souroundTile == resetTile && playerMove)
                        {
                            var i = Array.FindIndex(setAbleTiles, entity => entity == clickedTile);
                            if (i < shadowSetAbleTiles.Length)
                                _tilemap.SetTile(sourrendTilePos.Item1, shadowSetAbleTiles[i]);
                        }
                    }

                    foreach (var sourrendTilePos in GetSouroundCorr(cellPosition,
                                 currentDoorDir))
                    {
                        var souroundTile = _tilemap.GetTile(sourrendTilePos.Item1);

                        if (souroundTile == resetTile || shadowSetAbleTiles.Contains(souroundTile))
                        {
                            if (setAbleTiles.Contains(clickedTile))
                                _tilemap.SetTile(sourrendTilePos.Item1, clickedTile);
                            else
                                _tilemap.SetTile(sourrendTilePos.Item1, setAbleTiles[owner - 1]);
                        }
                        else
                        {
                            var shelledTileCard = cardShelled.FirstOrDefault(x => x.tile == souroundTile);
                            if (shelledTileCard != null && setAbleTiles.Contains(clickedTile))
                                _tilemap.SetTile(sourrendTilePos.Item1,
                                    shelledTileCard.inPlayerRangeTile[Array.IndexOf(setAbleTiles, clickedTile)]);

                            var shelledTileCardModif =
                                cardShelled.FirstOrDefault(x => x.inPlayerRangeTile.Contains(souroundTile));
                            if (shelledTileCardModif != null && setAbleTiles.Contains(clickedTile))
                                if (Array.IndexOf(setAbleTiles, clickedTile) !=
                                    Array.IndexOf(shelledTileCardModif.inPlayerRangeTile, souroundTile))
                                    _tilemap.SetTile(sourrendTilePos.Item1, shelledTileCardModif.inPlayerRangeTile[2]);
                        }
                    }
                }

                CreateRoom(cellPosition, card.roomtype, card.roomElement, currentDoorDir, owner, connectionForcing);

                if (playerMove)
                {
                    DdCodeEventHandler.Trigger_CardPlayed(card, isPlayer1Turn);
                    RemoveCardFromCardHolder();
                    DdCodeEventHandler.Trigger_NextPlayerTurn();
                    isPlayer1Turn = !isPlayer1Turn;
                    currentCard = null;
                }
                else
                {
                    DdCodeEventHandler.Trigger_PreSetCardSetOnTilemap(card, cellPosition);
                }

                var indicator =
                    Instantiate(indiactorDoor, _tilemap.CellToWorld(cellPosition), Quaternion.identity);
                if (indiactorDoorAnker == null) indiactorDoorAnker = GameObject.Find("IndicatorsAnker").transform;

                indicator.transform.parent = indiactorDoorAnker;
                indicator.GetComponent<DoorIndicator>().SetDoorIndiactor(currentDoorDir);
                if (connectionForcing) indicator.GetComponent<DoorIndicator>().OverExtend(overriteCurrentDoorDir);

                return true;
            }

            Debug.Log("Denied_NotRightRoation");
            return false;
        }


        private bool CheckConnectAblity(Tuple<Vector3Int, ConnectionDir>[] sourroundCorr)
        {
            // sourroundCorr Being an Tuple might overcomplicated , but tried solutation had edge cases where they failed
            // Reduce to relvant element so fewer opertion with find later 
            var filteredList = connectCollector.GetFullRoomList()
                .Where(item => sourroundCorr.Any(tuple => tuple.Item1 == item.Item1)).ToList();

            if (filteredList.Count <= 0) return false;

            foreach (var infoSourround in sourroundCorr)
            {
                var room = filteredList.Find(tuple => tuple.Item1 == infoSourround.Item1);
                if (room != null)
                    if (room.Item2.AllowedDoors.Contains(infoSourround.Item2.GetInvert()))
                        return true;
            }

            return false;
        }

        private void RemoveCardFromCardHolder()
        {
            Destroy(displayCardUi.gameObject);
        }

        private void CreateRoom(Vector3Int clickedTile, RoomType type, RoomElement element, bool[] allowedDoors,
            int owner, bool forceOnRoom)
        {
            var aroundpos = GetSouroundCorr(clickedTile); // 

            var establishConnection = connectCollector.GetPossibleConnects(aroundpos, allowedDoors, forceOnRoom);

            var conncection = new List<RoomConnection>();
            var newConnectionDir = new List<ConnectionDir>();

            for (var i = 0; i < establishConnection.Length; i++)
            {
                if (establishConnection[i] != -1) //All used
                    conncection.Add(new RoomConnection(establishConnection[i], (ConnectionDir)i));

                if (allowedDoors[i]) // all possible 
                    newConnectionDir.Add((ConnectionDir)i);
            }

            connectCollector.AddRoom(clickedTile, conncection, type, element, newConnectionDir, owner);
        }

        private Vector3Int[] GetSouroundCorr(Vector3Int clickedTile)
        {
            var aroundpos = new Vector3Int[6];

            var offsets = GetOffsetsCorrd(ref clickedTile);

            for (var i = 0; i < offsets.Length; i++) aroundpos[i] = clickedTile + offsets[i];

            return aroundpos;
        }

        private Vector3Int[] GetOffsetsCorrd(ref Vector3Int clickedTile)
        {
            return clickedTile.y % 2 == 0 ? _aroundHexDiffVectorEven : _aroundHexDiffVectorOdd;
        }

        private Tuple<Vector3Int, ConnectionDir>[] GetSouroundCorr(Vector3Int clickedTile, bool[] setDirections)
        {
            var aroundpos = new List<Tuple<Vector3Int, ConnectionDir>>();

            var offsets = GetOffsetsCorrd(ref clickedTile);

            for (var i = 0; i < offsets.Length; i++)
                if (setDirections[i])
                    aroundpos.Add(new Tuple<Vector3Int, ConnectionDir>(clickedTile + offsets[i], (ConnectionDir)i));

            return aroundpos.ToArray();
        }

        public void ChangeCard(DisplayCard newcurrentCardUi)
        {
            displayCardUi = newcurrentCardUi;

            currentCard = newcurrentCardUi is not null ? newcurrentCardUi.card : null;
            currentDoorDir = newcurrentCardUi is not null ? currentCard.GetAllowedDirection() : null;
        }

        public bool[] ShiftRight(bool[] array)
        {
            bool[] coveredClockwiese = { array[1], array[3], array[5], array[4], array[2], array[0] };

            // Create a new array with the same size
            var shiftedArray = new bool[coveredClockwiese.Length];

            // Shift the elements to the right
            for (var i = 0; i < coveredClockwiese.Length - 1; i++) shiftedArray[i + 1] = coveredClockwiese[i];

            // Move the last element to the first position
            shiftedArray[0] = coveredClockwiese[coveredClockwiese.Length - 1];

            shiftedArray = new[]
            {
                shiftedArray[5], shiftedArray[0], shiftedArray[4], shiftedArray[1], shiftedArray[3], shiftedArray[2]
            };

            return shiftedArray;
        }

        public void ShiftRightInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                currentDoorDir = ShiftRight(currentDoorDir);
                displayCardUi?.UpdateDirectionIndicator(currentDoorDir); // already ref so not done per comning Event

                DdCodeEventHandler.Trigger_CardRotating(currentDoorDir);
            }
        }

        public bool CheckConnectAblity(Card card, Vector3Int cellPosition)
        {
            var directions = card.GetAllowedDirection();
            var sourroundCorr = GetSouroundCorr(cellPosition, directions);
            return CheckConnectAblity(sourroundCorr);
        }

        private void FinalizePlacement()
        {
            if (_turnManager != null)
            {
                var cardToHand = _turnManager.isPlayer1Turn ? _turnManager.handPlayer1 : _turnManager.handPlayer2;
                if (cardToHand != null)
                {
                    //cardToHand.ReactivateHandCards();
                }

                if (_hexgridController != null) _hexgridController.ResetNavigation();
            }
        }
    }
}