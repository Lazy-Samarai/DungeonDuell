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
        public TileBase resetTile;

        public Card[] SpawnInfo;

        public Card[] WorldCard;

        public ConnectionsCollector connectCollector;

        public GameObject StartTiles;

        [Header("Player Objects")]

        public CardToHand HandPlayer1;

        public CardToHand HandPlayer2;

        public DiscardPile discardPile;

        public DiscardPile discardPile2;
       
        public bool Player_1Turn = true;


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
            /// So important Compentence of a return connectCollector (Round 2 and so On) are catched here
            connectCollector = FindObjectOfType<ConnectionsCollector>();
            StartTiles = FindObjectOfType<StartTilesGen>().gameObject;
            tilemap = FindObjectOfType<Tilemap>();
            ///

            Transform[] transformsSpwans = StartTiles.transform.GetChild(0).GetComponentsInChildren<Transform>().Skip(1).ToArray<Transform>(); // jump over parent
     
            for (int i = 0; i < transformsSpwans.Length; i++)
            {
                Transform transform = transformsSpwans[i]; 
                SpawnTile(transform.position, SpawnInfo[i],false);
            }

            Transform[] transformsWorld = StartTiles.transform.GetChild(1).GetComponentsInChildren<Transform>().Skip(1).ToArray<Transform>();
            for (int i = 0; i < transformsWorld.Length; i++)
            {
                print(transformsWorld[i].name);
                Transform transform = transformsWorld[i];
                SpawnTile(transform.position, WorldCard[i],false);
            }



        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {

                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));
                SpawnTile(mouseWorldPos, currentCard,true);

            }
        }

        private void SpawnTile(Vector3 mouseWorldPos, Card card,bool PlayerMove)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(mouseWorldPos.x, mouseWorldPos.y, cam.transform.position.z));

            TileBase clickedTile = tilemap.GetTile(cellPosition);

            if (clickedTile == resetTile & currentCard != null)
            {
                Debug.Log("Tile clicked at position: " + cellPosition);
                tilemap.SetTile(cellPosition, card.Tile);
                CreateRoom(cellPosition, card.roomtype);

                if (PlayerMove)
                {
                    // Karte zum Abwurfstapel hinzufügen und vom CardHolder entfernen    
                    discardPile.AddCardToDiscardPile(card);
                    RemoveCardFromCardHolder(Player_1Turn);
                    ChangePlayer(Player_1Turn);
                    Player_1Turn = !Player_1Turn;
                    currentCard = null;

                }
            }
            else
            {
                Debug.Log("Denied");
            }
        }
     
        private void ChangePlayer(bool Player_1Turn)
        {

            HandPlayer1.ShowHideDeck(Player_1Turn);
            HandPlayer2.ShowHideDeck(!Player_1Turn);       
        }

        private void RemoveCardFromCardHolder(bool player1)
        {
            Transform cardHolder = ((player1) ? HandPlayer1.transform.GetChild(0) : HandPlayer2.transform.GetChild(0));

            if (cardHolder.transform.childCount > 0)
            {
                Transform cardOnHolder = cardHolder.transform.GetChild(0);
                DisplayCard cardOnHolderScript = cardOnHolder.GetComponent<DisplayCard>();

                print("----------------");
                print(cardOnHolderScript);
                
                if (cardOnHolderScript != null)
                {
                    Card cardToDiscard = cardOnHolderScript.card;
                    discardPile.AddCardToDiscardPile(cardToDiscard);

                    // Karte vom CardHolder entfernen
                    Destroy(cardOnHolder.gameObject);
                }
            }

        }

        private void CreateRoom(Vector3Int clickedTile,RoomType type)
        {
            Vector3Int[] aroundpos = new Vector3Int[6];

            var offsets = (clickedTile.y % 2 == 0) ? aroundHexDiffVectorEVEN : aroundHexDiffVectorODD;

            for (int i = 0; i < offsets.Length; i++)
            {
                aroundpos[i] = clickedTile + offsets[i];
            }
            

            int[] establishConnection = connectCollector.GetPossibleConnects(aroundpos);

            List<RoomConnection> Conncection = new List<RoomConnection>();

            for (int i = 0; i < establishConnection.Length; i++)
            {
                if(establishConnection[i] != -1)
                {
                    Conncection.Add(new RoomConnection(establishConnection[i], (ConnectionDir)i));
                }
               
            }

            connectCollector.AddRoom(clickedTile, Conncection, type);

        }
        public void ChangeCard(Card newCard)
        {
            currentCard = newCard;
        }

    }
}
