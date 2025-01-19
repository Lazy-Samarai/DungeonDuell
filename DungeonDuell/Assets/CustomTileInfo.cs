using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    [CreateAssetMenu(fileName = "InfoTile", menuName = "Tiles/InfoTile")]
    public class CustomTileInfo : Tile  // or TileBase or RuleTile or other
    {
        // will be able to plug in value you want in Inspector for asset
        public int owner;
        public bool contested = false;

       
    }
}
