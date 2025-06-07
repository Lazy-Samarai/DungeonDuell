using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace dungeonduell
{
    public class SpawnWorldTiles : MonoBehaviour
    {
        [FormerlySerializedAs("SpawnInfo")] public Card[] spawnInfo;
        [FormerlySerializedAs("WorldCard")] public Card[] worldCard;
        private GameObject _startTiles;
        private TileClickHandler _tileClickHandler;

        private void Start()
        {
            _startTiles = transform.GetChild(0).gameObject;
            _tileClickHandler = FindFirstObjectByType<TileClickHandler>();
            SpawnTiles();
        }

        public void SpawnTiles()
        {
            var transformsSpwans = _startTiles.transform.GetChild(0).GetComponentsInChildren<Transform>().Skip(1)
                .ToArray(); // jump over parent

            for (var i = 0; i < transformsSpwans.Length; i++)
            {
                var transform = transformsSpwans[i];
                _tileClickHandler.SpawnTile(transform.position, spawnInfo[i], false, true, i + 1);
            }

            var transformsWorld = _startTiles.transform.GetChild(1).GetComponentsInChildren<Transform>().Skip(1)
                .ToArray();
            for (var i = 0; i < transformsWorld.Length; i++)
            {
                var transform = transformsWorld[i];
                _tileClickHandler.SpawnTile(transform.position, worldCard[0], false, false, 0);
            }
        }
    }
}