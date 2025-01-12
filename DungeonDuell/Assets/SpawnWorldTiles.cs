using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace dungeonduell
{
    public class SpawnWorldTiles : MonoBehaviour
    {
        GameObject StartTiles;
        TileClickHandler tileClickHandler;
        public Card[] SpawnInfo;
        public Card[] WorldCard;  

        void Start()
        {
            StartTiles = FindObjectOfType<StartTilesGen>().gameObject;
            tileClickHandler = FindObjectOfType<TileClickHandler>();
            SpawnTiles();
            foreach (Card card in WorldCard)
            {
                if (card.SheelCard)
                {
                    tileClickHandler.AddShellCardTypeToCheck(card);
                }
            }          

        }
        public void SpawnTiles()
        {
            Transform[] transformsSpwans = StartTiles.transform.GetChild(0).GetComponentsInChildren<Transform>().Skip(1).ToArray<Transform>(); // jump over parent

            for (int i = 0; i < transformsSpwans.Length; i++)
            {
                Transform transform = transformsSpwans[i];
                tileClickHandler.SpawnTile(transform.position, SpawnInfo[i], false, true);
            }

            Transform[] transformsWorld = StartTiles.transform.GetChild(1).GetComponentsInChildren<Transform>().Skip(1).ToArray<Transform>();
            for (int i = 0; i < transformsWorld.Length; i++)
            {
                print(transformsWorld[i].name);
                Transform transform = transformsWorld[i];
                tileClickHandler.SpawnTile(transform.position, WorldCard[0], false, false);
            }
        }

       
    }
}
