using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    [CreateAssetMenu]
    public class CardDistributor : ScriptableObject
    {
        [SerializeField] Card card;
        [SerializeField] int amount =1;

        public Card Card { get => card; set => card = value; }
        public int Amount { get => amount; set => amount = value; }
    }
}
