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
            StartTiles = transform.GetChild(0).gameObject;
            tileClickHandler = FindFirstObjectByType<TileClickHandler>();
            SpawnTiles();
        }

        public void SpawnTiles()
        {
            Transform[] transformsSpwans = StartTiles.transform.GetChild(0).GetComponentsInChildren<Transform>().Skip(1)
                .ToArray<Transform>(); // jump over parent

            for (int i = 0; i < transformsSpwans.Length; i++)
            {
                Transform transform = transformsSpwans[i];
                tileClickHandler.SpawnTile(transform.position, SpawnInfo[i], false, true, i + 1);
            }

            Transform[] transformsWorld = StartTiles.transform.GetChild(1).GetComponentsInChildren<Transform>().Skip(1)
                .ToArray<Transform>();
            for (int i = 0; i < transformsWorld.Length; i++)
            {
                Transform transform = transformsWorld[i];
                tileClickHandler.SpawnTile(transform.position, WorldCard[0], false, false, 0);
            }
        }
    }
}