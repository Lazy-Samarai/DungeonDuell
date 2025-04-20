using System.Linq;
using UnityEngine;

namespace dungeonduell
{
    public class SpawnWorldTiles : MonoBehaviour
    {
        public Card[] SpawnInfo;
        public Card[] WorldCard;
        private GameObject StartTiles;
        private TileClickHandler tileClickHandler;

        private void Start()
        {
            StartTiles = transform.GetChild(0).gameObject;
            tileClickHandler = FindFirstObjectByType<TileClickHandler>();
            SpawnTiles();
        }

        public void SpawnTiles()
        {
            var transformsSpwans = StartTiles.transform.GetChild(0).GetComponentsInChildren<Transform>().Skip(1)
                .ToArray(); // jump over parent

            for (var i = 0; i < transformsSpwans.Length; i++)
            {
                var transform = transformsSpwans[i];
                tileClickHandler.SpawnTile(transform.position, SpawnInfo[i], false, true, i + 1);
            }

            var transformsWorld = StartTiles.transform.GetChild(1).GetComponentsInChildren<Transform>().Skip(1)
                .ToArray();
            for (var i = 0; i < transformsWorld.Length; i++)
            {
                var transform = transformsWorld[i];
                tileClickHandler.SpawnTile(transform.position, WorldCard[0], false, false, 0);
            }
        }
    }
}