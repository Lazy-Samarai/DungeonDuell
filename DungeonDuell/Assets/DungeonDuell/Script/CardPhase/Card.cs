using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace dungeonduell
{
    [CreateAssetMenu]
    public class Card : ScriptableObject
    {
        public int cardId;
        public string cardName;
        public string cardDescription;

        public RoomType roomtype = RoomType.Generic;
        public RoomElement roomElement = RoomElement.Standard;
        public TileBase Tile;

        [System.Serializable]
        public struct DirBarSet { public bool TopLeft; public bool TopRight; public bool Left; public bool Right; public bool BottonLeft; public bool BottonRight; } // Going Around
        public DirBarSet startDoorConcellation;
        public bool[] GetAllowedDirection()
        {
            return new bool[] { startDoorConcellation.TopLeft, startDoorConcellation.TopRight, startDoorConcellation.Left, startDoorConcellation.Right, startDoorConcellation.BottonLeft, startDoorConcellation.BottonRight, };
        }

        public override bool Equals(object obj)
        {
            return obj is Card card &&
                   base.Equals(obj) &&
                   name == card.name &&
                   hideFlags == card.hideFlags &&
                   cardId == card.cardId &&
                   cardName == card.cardName &&
                   cardDescription == card.cardDescription &&
                   roomtype == card.roomtype &&
                   roomElement == card.roomElement &&
                   EqualityComparer<TileBase>.Default.Equals(Tile, card.Tile) &&
                   EqualityComparer<DirBarSet>.Default.Equals(startDoorConcellation, card.startDoorConcellation);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(base.GetHashCode());
            hash.Add(name);
            hash.Add(hideFlags);
            hash.Add(cardId);
            hash.Add(cardName);
            hash.Add(cardDescription);
            hash.Add(roomtype);
            hash.Add(roomElement);
            hash.Add(Tile);
            hash.Add(startDoorConcellation);
            return hash.ToHashCode();
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
