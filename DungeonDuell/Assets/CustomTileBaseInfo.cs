using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    public class CustomTileBaseInfo : TileBase
    {
        public int owner;
        public bool contested = false;
        public CustomTileBaseInfo(int owner, bool contested) 
        {
            this.owner = owner;
            this.contested = contested;
        }
    }
}
