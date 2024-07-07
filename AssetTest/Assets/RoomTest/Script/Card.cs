using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    [CreateAssetMenu]
    public class Card : ScriptableObject
    {
        public RoomType roomtype = RoomType.Generic;
        public TileBase Tile;
    }
}
