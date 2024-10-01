using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    [CreateAssetMenu]
    public class Card : ScriptableObject
    {
        public int cardId;
        public string cardName;
        public string cardDescription;
        public Sprite cardImage;

        public RoomType roomtype = RoomType.Generic;
        public RoomElement roomElement = RoomElement.Standard;
        public TileBase Tile;
    }
}
