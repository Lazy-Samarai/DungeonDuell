using UnityEngine;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    [CreateAssetMenu]
    public class ShellCard : Card // Shell card should have a unique Tile!
    {
        public TileBase[] InPlayerRangeTile;

        public TileBase CompleteTile;
    }
}