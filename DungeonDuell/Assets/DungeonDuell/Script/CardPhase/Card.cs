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

        [System.Serializable]
        public struct DirBarSet {  public bool TopLeft; public bool TopRight; public bool Left; public bool Right; public bool BottonLeft; public bool BottonRight; } // Going Around
        public DirBarSet startDoorConcellation;
        public bool[] GetAllowedDirection()
        {
            return new bool[]{ startDoorConcellation.TopLeft, startDoorConcellation.TopRight, startDoorConcellation.Left, startDoorConcellation.Right, startDoorConcellation.BottonLeft, startDoorConcellation.BottonRight, };
        }
    }
}
