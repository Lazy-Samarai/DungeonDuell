using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

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
        [FormerlySerializedAs("Tile")] public TileBase tile;
        public DirBarSet startDoorConcellation;
        public bool isBridgeCard = false;

        public bool[] GetAllowedDirection()
        {
            return new[]
            {
                startDoorConcellation.topLeft, startDoorConcellation.topRight, startDoorConcellation.left,
                startDoorConcellation.right, startDoorConcellation.bottonLeft, startDoorConcellation.bottonRight
            };
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
                   EqualityComparer<TileBase>.Default.Equals(tile, card.tile) &&
                   EqualityComparer<DirBarSet>.Default.Equals(startDoorConcellation, card.startDoorConcellation);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(base.GetHashCode());
            hash.Add(name);
            hash.Add(hideFlags);
            hash.Add(cardId);
            hash.Add(cardName);
            hash.Add(cardDescription);
            hash.Add(roomtype);
            hash.Add(roomElement);
            hash.Add(tile);
            hash.Add(startDoorConcellation);
            return hash.ToHashCode();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        [Serializable]
        public struct DirBarSet
        {
            [FormerlySerializedAs("TopLeft")] public bool topLeft;
            [FormerlySerializedAs("TopRight")] public bool topRight;
            [FormerlySerializedAs("Left")] public bool left;
            [FormerlySerializedAs("Right")] public bool right;
            [FormerlySerializedAs("BottonLeft")] public bool bottonLeft;
            [FormerlySerializedAs("BottonRight")] public bool bottonRight;
        } // Going Around
    }
}