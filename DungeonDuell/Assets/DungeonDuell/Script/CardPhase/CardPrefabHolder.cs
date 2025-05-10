using UnityEngine;

namespace dungeonduell
{
    public class CardPrefabHolder : MonoBehaviour
    {
        // As Both player needed is managed here only this need to be adjusted for future changes
        public GameObject[] cardPrefab;

        public GameObject GetCardPrefab(RoomType roomType)
        {
            switch (roomType)
            {
                case RoomType.Enemy:
                    return cardPrefab[2];
                case RoomType.NormalLott:
                    return cardPrefab[1];
                case RoomType.Generic:
                    return cardPrefab[0];
                default:
                    return cardPrefab[0];
            }
        }
    }
}