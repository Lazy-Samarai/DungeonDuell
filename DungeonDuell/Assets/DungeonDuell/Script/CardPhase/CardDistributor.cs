using UnityEngine;

namespace dungeonduell
{
    [CreateAssetMenu]
    public class CardDistributor : ScriptableObject
    {
        [SerializeField] private Card card;
        [SerializeField] private int amount = 1;

        public Card Card
        {
            get => card;
            set => card = value;
        }

        public int Amount
        {
            get => amount;
            set => amount = value;
        }
    }
}