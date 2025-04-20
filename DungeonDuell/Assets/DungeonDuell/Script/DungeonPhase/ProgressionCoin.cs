using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    /// <summary>
    ///     Coin manager
    /// </summary>
    [AddComponentMenu("TopDown Engine/Items/ProgressionCoin")]
    public class ProgressionCoin : PickableItem
    {
        /// The amount of points to add when collected
        [Tooltip("The amount of points to add when collected")]
        public int PointsToAdd = 1;


        /// <summary>
        ///     Triggered when something collides with the coin
        /// </summary>
        /// <param name="collider">Other.</param>
        protected override void Pick(GameObject picker)
        {
            //TopDownEnginePointEvent.Trigger(PointsMethods.Add, PointsToAdd);

            var character = picker.GetComponent<Character>();
            if (character != null)
                CoinEvent.Trigger(PointsToAdd, picker);
            else
                Debug.LogError("Character konnte nicht gefunden werden.");
        }
    }
}