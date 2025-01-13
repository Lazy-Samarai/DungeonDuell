using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;

namespace dungeonduell
{

    public class TileClickHandler : MonoBehaviour
    {
        public Camera cam;

        public Tilemap tilemap;

        public Card currentCard;
        public bool[] currentDoorDir = new bool[] { true, true, true, true, true, true };
        public DisplayCard displayCardUi;

        public GameObject indiactorDoor;
        public Transform indiactorDoorAnker;

        public TileBase resetTile;

        public TileBase[] setAbleTiles;

        public Card[] SpawnInfo;

        public Card[] WorldCard;

        public ConnectionsCollector connectCollector;

        public GameObject StartTiles;

        [Header("Player Objects")]

        public CardToHand HandPlayer1;

        public CardToHand HandPlayer2;

        public DiscardPile discardPile;

        public DiscardPile discardPile2;

        public TurnManager turnManager; // Referenz zum TurnManager

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
            StartTiles = FindObjectOfType<StartTilesGen>().gameObject;
            tilemap = FindObjectOfType<Tilemap>();
            turnManager = FindObjectOfType<TurnManager>(); // Finde den TurnManager

            Transform[] transformsSpwans = StartTiles.transform.GetChild(0).GetComponentsInChildren<Transform>().Skip(1).ToArray<Transform>(); // jump over parent

            for (int i = 0; i < transformsSpwans.Length; i++)
            {
                Transform transform = transformsSpwans[i];
                SpawnTile(transform.position, SpawnInfo[i], false,true,i+1);
            }

            Transform[] transformsWorld = StartTiles.transform.GetChild(1).GetComponentsInChildren<Transform>().Skip(1).ToArray<Transform>();
            for (int i = 0; i < transformsWorld.Length; i++)
            {
                print(transformsWorld[i].name);
                Transform transform = transformsWorld[i];
                SpawnTile(transform.position, WorldCard[0], false, false,0);
            }

        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));
                SpawnTile(mouseWorldPos, currentCard, true,true, turnManager.isPlayer1Turn ? 1 : 2);
            }
            if (Input.GetKeyDown(KeyCode.R)) // Test 
            {
                currentDoorDir = ShiftRight(currentDoorDir);
                displayCardUi?.UpdateDirectionIndicator(currentDoorDir); // this might be better be resolved with an event later 
            }
        }

        private void SpawnTile(Vector3 mouseWorldPos, Card card, bool PlayerMove,bool spawnSourroundSetables,int owner)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(mouseWorldPos.x, mouseWorldPos.y, cam.transform.position.z));

            TileBase clickedTile = tilemap.GetTile(cellPosition);

            if(clickedTile != resetTile | !PlayerMove)
            {
                bool[] OverriteCurrentDoorDir =  new bool[] { false, false, false, false, false, false };
                if ((setAbleTiles.Contains(clickedTile) | !PlayerMove) && currentCard != null)
                {
                    bool connectionForcing = false;
                    if(clickedTile == setAbleTiles[setAbleTiles.Length - 1]) // Hited Contested
                    {
                        connectionForcing = true;
                        Vector3Int[] offset = (cellPosition.y % 2 == 0) ? aroundHexDiffVectorEVEN : aroundHexDiffVectorODD;
                        for (int i = 0; i < offset.Length; i++)
                        {
                            if (connectCollector.GetFullRoomList().Any(entry => entry.Item1 == cellPosition + offset[i]))
                            {
                                currentDoorDir[i] = true; // Connect to all rooms that are there    
                                OverriteCurrentDoorDir[i] = true;                        
                            }
                        }
                    }
                                                                              
                    Tuple<Vector3Int, ConnectionDir>[] sourroundCorr = GetSouroundCorr(cellPosition, currentDoorDir);
                   
                    if (CheckConnectAblity(sourroundCorr) | !PlayerMove)
                    {
                        Debug.Log("Tile clicked at position: " + cellPosition);
                        // Set Tiles
                        // Main Spawn
                        tilemap.SetTile(cellPosition, card.Tile);

                       
                        //Sourround 
                        if (spawnSourroundSetables)
                        {
                            foreach (Tuple<Vector3Int, ConnectionDir> SourrendTilePos in GetSouroundCorr(cellPosition, new bool[] { true, true, true, true, true, true }))
                            {
                                TileBase souroundTile = tilemap.GetTile(SourrendTilePos.Item1);               
                                if(setAbleTiles.Contains(souroundTile))
                                {
                                    if (clickedTile != souroundTile)
                                    {
                                        tilemap.SetTile(SourrendTilePos.Item1, setAbleTiles[setAbleTiles.Length - 1]);
                                    }                              
                                   
                                } 
                            }

                            foreach (Tuple<Vector3Int, ConnectionDir> SourrendTilePos in GetSouroundCorr(cellPosition, currentDoorDir))
                            {
                                TileBase souroundTile = tilemap.GetTile(SourrendTilePos.Item1);                              
                                                
                                if(souroundTile == resetTile)
                                {
                                    if(setAbleTiles.Contains(clickedTile))
                                    {
                                        tilemap.SetTile(SourrendTilePos.Item1, clickedTile); 
                                    }
                                    else
                                    {
                                        tilemap.SetTile(SourrendTilePos.Item1,  setAbleTiles[owner - 1]); 
                                    }                                            
                                }                                                   
                                
                            }
                        }
                        

                        // Create Room Info
                        CreateRoom(cellPosition, card.roomtype, card.roomElement, currentDoorDir, owner,connectionForcing);

                        // Card Disposal
                        if (PlayerMove)
                        {
                            // Karte zum Abwurfstapel hinzufügen und vom CardHolder entfernen    
                            discardPile.AddCardToDiscardPile(card);
                            RemoveCardFromCardHolder(turnManager.isPlayer1Turn);
                            RemoveCardFromCardHolder(!turnManager.isPlayer1Turn);
                            turnManager.EndPlayerTurn(); // Übergib die Verantwortung an den TurnManager
                            currentCard = null;

                        }

                        // Set Indicator
                        GameObject indicator = Instantiate(indiactorDoor, tilemap.CellToWorld(cellPosition), Quaternion.identity);
                        if (indiactorDoorAnker == null)
                        {
                            indiactorDoorAnker = GameObject.Find("IndicatorsAnker").transform; // TODO HotFix ; Make better later

                        }
                        indicator.transform.parent = indiactorDoorAnker;
                        indicator.GetComponent<DoorIndicator>().SetDoorIndiactor(currentDoorDir);
                        if(connectionForcing)
                        {
                            indicator.GetComponent<DoorIndicator>().OverExtend(OverriteCurrentDoorDir);
                        }
                        
                       
                    }
                    else
                    {
                        // Some Visual Reaction here 
                    }

                    
                }
                else
                {
                    Debug.Log("Denied_SetOrNoCard");
                }
            }
            else
            {
                Debug.Log("OutOfReachTile");
            }
           
        }

        private bool CheckConnectAblity(Tuple<Vector3Int, ConnectionDir>[] sourroundCorr)
        {  
            // sourroundCorr Being an Tuple might overcomplicated , but tried solutation had edge cases where they failed
            // Reduce to relvant element so fewer opertion with find later 
            List<Tuple<Vector3Int, RoomInfo>> filteredList = connectCollector.GetFullRoomList().Where(item => sourroundCorr.Any(tuple => tuple.Item1 == item.Item1)).ToList();

            if(filteredList.Count <= 0)
            {
                return false;
            }

            foreach (Tuple<Vector3Int, ConnectionDir> InfoSourround in sourroundCorr)
            {
                Tuple<Vector3Int, RoomInfo> room = filteredList.Find(tuple => tuple.Item1 == InfoSourround.Item1);
                if(room != null)
                {
                    if (room.Item2.allowedDoors.Contains(InfoSourround.Item2.GetInvert()))
                    {
                        return true;
                    }
                }        
            }   
            return false;
        }

        private void RemoveCardFromCardHolder(bool player1)
        {
            Transform cardHolder = ((player1) ? HandPlayer1.transform.GetChild(0) : HandPlayer2.transform.GetChild(0));

            if (cardHolder.transform.childCount > 0)
            {
                if (displayCardUi != null)
                {
                    Card cardToDiscard = displayCardUi.card;
                    discardPile.AddCardToDiscardPile(cardToDiscard);

                    // Karte vom CardHolder entfernen
                    Destroy(displayCardUi.gameObject);
                }
            }
        }

        private void CreateRoom(Vector3Int clickedTile, RoomType type, RoomElement element, bool[] allowedDoors,int owner,bool forceOnRoom)
        {
            Vector3Int[] aroundpos = GetSouroundCorr(clickedTile); // 

            int[] establishConnection = connectCollector.GetPossibleConnects(aroundpos, allowedDoors,forceOnRoom);

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
                    //print(((ConnectionDir)i).ToString());
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

        private Tuple<Vector3Int,ConnectionDir>[] GetSouroundCorr(Vector3Int clickedTile,bool[] setDirections)
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



        public void ChangeCard(Card newCard, bool[] newcurrentDoorDir, DisplayCard newcurrentCardUi)
        {
            currentCard = newCard;
            currentDoorDir = newcurrentDoorDir;
            displayCardUi = newcurrentCardUi;
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
    }
}
