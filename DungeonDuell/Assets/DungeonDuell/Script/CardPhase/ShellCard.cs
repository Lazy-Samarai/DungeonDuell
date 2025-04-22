using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    [CreateAssetMenu]
    public class ShellCard : Card // Shell card should have a unique Tile!
    {
        [FormerlySerializedAs("InPlayerRangeTile")]
        public TileBase[] inPlayerRangeTile;

        [FormerlySerializedAs("CompleteTile")] public TileBase completeTile;
    }
}