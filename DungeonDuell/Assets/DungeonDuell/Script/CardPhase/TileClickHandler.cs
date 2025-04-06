using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Unity.VisualScripting;
using Cinemachine;

namespace dungeonduell
{
    public class TileClickHandler : MonoBehaviour, IObserver
    {
        public Camera cam;
        private Tilemap tilemap;
        [TagField]
        [SerializeField] private string TileMapTag;

        public Card currentCard;
        public bool[] currentDoorDir = new bool[] { true, true, true, true, true, true };
        public DisplayCard displayCardUi;
        public GameObject indiactorDoor;
        public Transform indiactorDoorAnker;
        public TileBase resetTile;
        public TileBase[] setAbleTiles;
        public TileBase[] shadowSetAbleTiles;
        public List<ShellCard> CardShelled;
        public ConnectionsCollector connectCollector;
        public bool isPlayer1Turn = true;
        private TurnManager turnManager;
        private HexgridController hexgridController;

        Vector3Int[] aroundHexDiffVectorEVEN = {
            new Vector3Int(-1, 1), // TopLeft
            new Vector3Int(0, 1), // TopRight

            new Vector3Int(-1, 0), // left
            new Vector3Int(1, 0), // right 

            new Vector3Int(-1, -1), // BottonLeft
            new Vector3Int(0, -1), // BottonRight 
        };
        Vector3Int[] aroundHexDiffVectorODD = {
            new Vector3Int(0, 1), // TopLeft
            new Vector3Int(1, 1), // TopRight

            new Vector3Int(-1, 0), // left
            new Vector3Int(1, 0), // right 

            new Vector3Int(0, -1), // BottonLeft
            new Vector3Int(1, -1), // BottonRight 
        };


        private void Start()
        {
            connectCollector = FindObjectOfType<ConnectionsCollector>();
            tilemap = FindObjectsOfType<Tilemap>().FirstOrDefault(tm => tm.gameObject.tag == TileMapTag); // Becuase there is also the hovermap
            turnManager = FindObjectOfType<TurnManager>();
            hexgridController = FindObjectOfType<HexgridController>();
        }

        void Update()
        {
            if (currentCard != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));
                    SpawnTile(mouseWorldPos, currentCard, true, true, isPlayer1Turn ? 1 : 2);
                }
            }
        }

        public bool IsSetablePosition(Vector3Int cellPosition)
        {
            TileBase tile = tilemap.GetTile(cellPosition);
            if (tile == null) return false; // kein Tile = ungültig

            // Gültig, wenn NICHT das resetTile und in setAbleTiles enthalten
            if (tile != resetTile && (setAbleTiles.Contains(tile) | CardShelled.Any(card => card.InPlayerRangeTile.Contains(tile))))
            {
                return true;
            }
            return false;
        }



        public bool SpawnTile(Vector3 mouseWorldPos, Card card, bool PlayerMove, bool spawnSourroundSetables, int owner)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(mouseWorldPos.x, mouseWorldPos.y, cam.transform.position.z));
            TileBase clickedTile = tilemap.GetTile(cellPosition);

            // Sonderfall: ShellCard
            if (card is ShellCard)
            {
                if (card.Tile is AnimatedTile)
                {
                    tilemap.SetTile(cellPosition, null);
                }
                tilemap.SetTile(cellPosition, card.Tile);

                FinalizePlacement();
                return true;
            }

            if (clickedTile != resetTile || !PlayerMove)
            {
                // ShellCard-Verarbeitung
                ShellCard shelledTileCard = CardShelled.FirstOrDefault(x => x.InPlayerRangeTile.Contains(clickedTile));
                if (shelledTileCard != null)
                {
                    int index = Array.FindIndex(shelledTileCard.InPlayerRangeTile, tile => clickedTile == tile);
                    clickedTile = setAbleTiles[index];
                    shelledTileCard.startDoorConcellation = card.startDoorConcellation;

                    card = (Card)shelledTileCard.Clone();
                    card.Tile = shelledTileCard.CompleteTile;
                }

                // Nur fortfahren, wenn gültig
                if ((setAbleTiles.Contains(clickedTile) && currentCard != null) || !PlayerMove)
                {
                    bool wasHandled = CardUsingHandling(card, PlayerMove, spawnSourroundSetables, cellPosition, clickedTile, owner);

                    if (wasHandled)
                    {
                        FinalizePlacement();
                        return true;
                    }
                    else
                    {
                        Debug.Log("[TileClickHandler] Platzierung durch CardUsingHandling abgelehnt");
                        return false;
                    }
                }
                else
                {
                    Debug.Log("[TileClickHandler] Denied_SetOrNoCard");
                    return false;
                }
            }
            else
            {
                Debug.Log("[TileClickHandler] OutOfReachTile");
                return false;
            }
        }




        private bool CardUsingHandling(Card card, bool PlayerMove, bool spawnSourroundSetables, Vector3Int cellPosition, TileBase clickedTile, int owner)
        {
            bool[] OverriteCurrentDoorDir = new bool[] { false, false, false, false, false, false };
            bool connectionForcing = false;

            if (clickedTile == setAbleTiles[setAbleTiles.Length - 1]) // Hited Contested
            {
                DDCodeEventHandler.Trigger_DungeonConnected();
                connectionForcing = true;
                Vector3Int[] offset = (cellPosition.y % 2 == 0) ? aroundHexDiffVectorEVEN : aroundHexDiffVectorODD;
                for (int i = 0; i < offset.Length; i++)
                {
                    if (connectCollector.GetFullRoomList().Any(entry => entry.Item1 == cellPosition + offset[i]))
                    {
                        currentDoorDir[i] = true;
                        OverriteCurrentDoorDir[i] = true;
                    }
                }
            }

            Tuple<Vector3Int, ConnectionDir>[] sourroundCorr = GetSouroundCorr(cellPosition, currentDoorDir);

            if (CheckConnectAblity(sourroundCorr) || !PlayerMove)
            {
                // --- HIER bleibt alles wie im Original ---
                tilemap.SetTile(cellPosition, card.Tile);

                if (spawnSourroundSetables)
                {
                    foreach (Tuple<Vector3Int, ConnectionDir> SourrendTilePos in GetSouroundCorr(cellPosition, new bool[] { true, true, true, true, true, true }))
                    {
                        TileBase souroundTile = tilemap.GetTile(SourrendTilePos.Item1);

                        if (setAbleTiles.Contains(souroundTile))
                        {
                            if (clickedTile != souroundTile)
                            {
                                tilemap.SetTile(SourrendTilePos.Item1, setAbleTiles[setAbleTiles.Length - 1]);
                            }
                        }
                        else if (shadowSetAbleTiles.Contains(souroundTile))
                        {
                            int i = Array.FindIndex(setAbleTiles, entity => entity == clickedTile);
                            if (i == setAbleTiles.Length - 1)
                            {
                                tilemap.SetTile(SourrendTilePos.Item1, setAbleTiles[setAbleTiles.Length - 1]);
                            }
                            else if (souroundTile != shadowSetAbleTiles[i])
                            {
                                tilemap.SetTile(SourrendTilePos.Item1, setAbleTiles[setAbleTiles.Length - 1]);
                            }
                        }

                        if (souroundTile == resetTile && PlayerMove)
                        {
                            int i = Array.FindIndex(setAbleTiles, entity => entity == clickedTile);
                            if (i < shadowSetAbleTiles.Length)
                            {
                                tilemap.SetTile(SourrendTilePos.Item1, shadowSetAbleTiles[i]);
                            }
                        }
                    }

                    foreach (Tuple<Vector3Int, ConnectionDir> SourrendTilePos in GetSouroundCorr(cellPosition, currentDoorDir))
                    {
                        TileBase souroundTile = tilemap.GetTile(SourrendTilePos.Item1);

                        if (souroundTile == resetTile || shadowSetAbleTiles.Contains(souroundTile))
                        {
                            if (setAbleTiles.Contains(clickedTile))
                            {
                                tilemap.SetTile(SourrendTilePos.Item1, clickedTile);
                            }
                            else
                            {
                                tilemap.SetTile(SourrendTilePos.Item1, setAbleTiles[owner - 1]);
                            }
                        }
                        else
                        {
                            ShellCard shelledTileCard = CardShelled.FirstOrDefault(x => x.Tile == souroundTile);
                            if (shelledTileCard != null && setAbleTiles.Contains(clickedTile))
                            {
                                tilemap.SetTile(SourrendTilePos.Item1, shelledTileCard.InPlayerRangeTile[Array.IndexOf(setAbleTiles, clickedTile)]);
                            }

                            ShellCard shelledTileCardModif = CardShelled.FirstOrDefault(x => x.InPlayerRangeTile.Contains(souroundTile));
                            if (shelledTileCardModif != null && setAbleTiles.Contains(clickedTile))
                            {
                                if (Array.IndexOf(setAbleTiles, clickedTile) != Array.IndexOf(shelledTileCardModif.InPlayerRangeTile, souroundTile))
                                {
                                    tilemap.SetTile(SourrendTilePos.Item1, shelledTileCardModif.InPlayerRangeTile[2]);
                                }
                            }
                        }
                    }
                }

                CreateRoom(cellPosition, card.roomtype, card.roomElement, currentDoorDir, owner, connectionForcing);

                if (PlayerMove)
                {
                    DDCodeEventHandler.Trigger_CardPlayed(card, isPlayer1Turn);
                    RemoveCardFromCardHolder();
                    DDCodeEventHandler.Trigger_NextPlayerTurn();
                    isPlayer1Turn = !isPlayer1Turn;
                    currentCard = null;
                }
                else{
                    DDCodeEventHandler.Trigger_PreSetCardSetOnTilemap(card,cellPosition);
                }

                GameObject indicator = Instantiate(indiactorDoor, tilemap.CellToWorld(cellPosition), Quaternion.identity);
                if (indiactorDoorAnker == null)
                {
                    indiactorDoorAnker = GameObject.Find("IndicatorsAnker").transform;
                }
                indicator.transform.parent = indiactorDoorAnker;
                indicator.GetComponent<DoorIndicator>().SetDoorIndiactor(currentDoorDir);
                if (connectionForcing)
                {
                    indicator.GetComponent<DoorIndicator>().OverExtend(OverriteCurrentDoorDir);
                }

                return true;
            }
            else
            {
                Debug.Log("Denied_NotRightRoation");
                return false;
            }
        }


        private bool CheckConnectAblity(Tuple<Vector3Int, ConnectionDir>[] sourroundCorr)
        {
            // sourroundCorr Being an Tuple might overcomplicated , but tried solutation had edge cases where they failed
            // Reduce to relvant element so fewer opertion with find later 
            List<Tuple<Vector3Int, RoomInfo>> filteredList = connectCollector.GetFullRoomList().Where(item => sourroundCorr.Any(tuple => tuple.Item1 == item.Item1)).ToList();

            if (filteredList.Count <= 0)
            {
                return false;
            }

            foreach (Tuple<Vector3Int, ConnectionDir> InfoSourround in sourroundCorr)
            {
                Tuple<Vector3Int, RoomInfo> room = filteredList.Find(tuple => tuple.Item1 == InfoSourround.Item1);
                if (room != null)
                {
                    if (room.Item2.allowedDoors.Contains(InfoSourround.Item2.GetInvert()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void RemoveCardFromCardHolder()
        {
            Destroy(displayCardUi.gameObject);
        }

        private void CreateRoom(Vector3Int clickedTile, RoomType type, RoomElement element, bool[] allowedDoors, int owner, bool forceOnRoom)
        {
            Vector3Int[] aroundpos = GetSouroundCorr(clickedTile); // 

            int[] establishConnection = connectCollector.GetPossibleConnects(aroundpos, allowedDoors, forceOnRoom);

            List<RoomConnection> Conncection = new List<RoomConnection>();
            List<ConnectionDir> newConnectionDir = new List<ConnectionDir>();

            for (int i = 0; i < establishConnection.Length; i++)
            {
                if (establishConnection[i] != -1) //All used
                {
                    Conncection.Add(new RoomConnection(establishConnection[i], (ConnectionDir)i));
                }
                if (allowedDoors[i]) // all possible 
                {
                    newConnectionDir.Add((ConnectionDir)i);
                }
            }
            connectCollector.AddRoom(clickedTile, Conncection, type, element, newConnectionDir, owner);
        }

        private Vector3Int[] GetSouroundCorr(Vector3Int clickedTile)
        {
            Vector3Int[] aroundpos = new Vector3Int[6];

            Vector3Int[] offsets = GetOffsetsCorrd(ref clickedTile);

            for (int i = 0; i < offsets.Length; i++)
            {
                aroundpos[i] = clickedTile + offsets[i];
            }

            return aroundpos;
        }

        private Vector3Int[] GetOffsetsCorrd(ref Vector3Int clickedTile)
        {
            return (clickedTile.y % 2 == 0) ? aroundHexDiffVectorEVEN : aroundHexDiffVectorODD;
        }

        private Tuple<Vector3Int, ConnectionDir>[] GetSouroundCorr(Vector3Int clickedTile, bool[] setDirections)
        {
            List<Tuple<Vector3Int, ConnectionDir>> aroundpos = new List<Tuple<Vector3Int, ConnectionDir>>();

            var offsets = GetOffsetsCorrd(ref clickedTile);

            for (int i = 0; i < offsets.Length; i++)
            {
                if (setDirections[i])
                {
                    aroundpos.Add(new Tuple<Vector3Int, ConnectionDir>(clickedTile + offsets[i], (ConnectionDir)i));
                }

            }

            return aroundpos.ToArray();
        }
        public void ChangeCard(DisplayCard newcurrentCardUi)
        {
            displayCardUi = newcurrentCardUi;

            currentCard = (newcurrentCardUi is not null) ? newcurrentCardUi.card : null;
            currentDoorDir = (newcurrentCardUi is not null) ? currentCard.GetAllowedDirection() : null;
        }

        public bool[] ShiftRight(bool[] array)
        {
            bool[] coveredClockwiese = { array[1], array[3], array[5], array[4], array[2], array[0] };

            // Create a new array with the same size
            bool[] shiftedArray = new bool[coveredClockwiese.Length];

            // Shift the elements to the right
            for (int i = 0; i < (coveredClockwiese.Length - 1); i++)
            {
                shiftedArray[i + 1] = coveredClockwiese[i];
            }

            // Move the last element to the first position
            shiftedArray[0] = coveredClockwiese[coveredClockwiese.Length - 1];

            shiftedArray = new bool[] { shiftedArray[5], shiftedArray[0], shiftedArray[4], shiftedArray[1], shiftedArray[3], shiftedArray[2] };

            return shiftedArray;
        }

        public void ShiftRightInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                currentDoorDir = ShiftRight(currentDoorDir);
                displayCardUi?.UpdateDirectionIndicator(currentDoorDir); // already ref so not done per comning Event

                DDCodeEventHandler.Trigger_CardRotating(currentDoorDir);
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
            if (turnManager != null)
            {
                CardToHand cardToHand = turnManager.isPlayer1Turn ? turnManager.HandPlayer1 : turnManager.HandPlayer2;
                if (cardToHand != null)
                {
                    //cardToHand.ReactivateHandCards();
                }

                if (hexgridController != null)
                {
                    hexgridController.ResetNavigation();
                }
            }
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
            DDCodeEventHandler.CardSelected += ChangeCard;
        }

        public void UnsubscribeToAllEvents()
        {
            DDCodeEventHandler.CardSelected -= ChangeCard;
        }
    }
}
