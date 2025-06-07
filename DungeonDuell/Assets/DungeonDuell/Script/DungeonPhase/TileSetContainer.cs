using UnityEngine;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    public class TileSetContainer : MonoBehaviour
    {
        public Tile tile;

        public Tile GetTile()
        {
            return tile;
        }
    }
}