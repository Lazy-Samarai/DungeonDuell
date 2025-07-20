using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    [CreateAssetMenu]
    public class ShellCard : Card // Shell card should have a unique Tile!
    {
        [FormerlySerializedAs("InPlayerRangeTile")]
        public GameObject marker;

        public TileBase[] completeTiles;

        public TileBase GetCompleteTile(RoomType roomType)
        {
            switch (roomType)
            {
                case RoomType.Enemy:
                    return completeTiles[1];
                case RoomType.NormalLott:
                    return completeTiles[2];
                default:
                    return completeTiles[0];
            }
        }
    }
}