using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    public class BlizzardSpeedTile : MonoBehaviour
    {
        public enum TriggerAndCollisionMask
        {
            IgnoreAll = 0,
            OnTriggerEnter = 1 << 0,
            OnTriggerStay = 1 << 1,
            OnTriggerExit2D = 1 << 6,
            OnTriggerStay2D = 1 << 7,

            All_3D = OnTriggerEnter | OnTriggerStay,
            All_2D = OnTriggerExit2D | OnTriggerStay2D,
            All = All_3D | All_2D
        }

        public const TriggerAndCollisionMask AllowedTriggerCallbacks = TriggerAndCollisionMask.OnTriggerEnter
                                                                       | TriggerAndCollisionMask.OnTriggerStay
                                                                       | TriggerAndCollisionMask.OnTriggerExit2D
                                                                       | TriggerAndCollisionMask.OnTriggerStay2D;

        [Tooltip("the layers that will be affected by this tile")]
        public LayerMask TargetLayerMask;

        [Tooltip("Defines on what triggers the effect should be applied")]
        public TriggerAndCollisionMask TriggerFilter = AllowedTriggerCallbacks;

        public float blizzardSpeedMultiplier = 0.5f; // Multiplier for movement speed in the Blizzard zone
        private CharacterRun characterRun;
        private bool isInBlizzard; // Tracks if the player is in the Blizzard zone

        private CharacterMovement playerMovement;
        private bool speedModified; // Tracks if speed has been modified

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerExit2D)) return;
            if ((TargetLayerMask.value & (1 << collision.gameObject.layer)) == 0) return;

            isInBlizzard = false;
            ApplyBlizzardEffect(collision.gameObject);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerStay2D)) return;
            if ((TargetLayerMask.value & (1 << collision.gameObject.layer)) == 0) return;

            isInBlizzard = true;
            ApplyBlizzardEffect(collision.gameObject);
        }

        protected virtual void ApplyBlizzardEffect(GameObject collision)
        {
            playerMovement = collision.GetComponent<CharacterMovement>();
            characterRun = collision.GetComponent<CharacterRun>();

            if (playerMovement != null && characterRun != null)
            {
                if (isInBlizzard && !speedModified)
                {
                    characterRun.RunStop();
                    characterRun.AbilityPermitted = false; // Verhindert Sprinten im Blizzard
                    playerMovement.EnterBlizzardZone(blizzardSpeedMultiplier); // Reduziert Geschwindigkeit
                    speedModified = true;
                }
                else if (!isInBlizzard && speedModified)
                {
                    playerMovement.ExitBlizzardZone(); // Setzt normale Geschwindigkeit wieder
                    characterRun.AbilityPermitted = true; // Sprinten wieder erlauben
                    speedModified = false;
                }
            }
        }
    }
}