using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

namespace dungeonduell
{
    /// <summary>
    ///     Coin manager
    /// </summary>
    [AddComponentMenu("TopDown Engine/Items/ProgressionCoin")]
    public class ProgressionCoin : PickableItem
    {
        /// The amount of points to add when collected
        [FormerlySerializedAs("PointsToAdd")] [Tooltip("The amount of points to add when collected")]
        public int pointsToAdd = 1;


        /// <summary>
        ///     Triggered when something collides with the coin
        /// </summary>
        /// <param name="collider">Other.</param>
        protected override void Pick(GameObject picker)
        {
            //TopDownEnginePointEvent.Trigger(PointsMethods.Add, PointsToAdd);

            var character = picker.GetComponent<Character>();
            if (character != null)
                CoinEvent.Trigger(pointsToAdd, picker);
            else
                Debug.LogError("Character konnte nicht gefunden werden.");
        }
    }
}