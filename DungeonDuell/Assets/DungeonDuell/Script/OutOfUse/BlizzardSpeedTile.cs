using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

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

            All3D = OnTriggerEnter | OnTriggerStay,
            All2D = OnTriggerExit2D | OnTriggerStay2D,
            All = All3D | All2D
        }

        public const TriggerAndCollisionMask AllowedTriggerCallbacks = TriggerAndCollisionMask.OnTriggerEnter
                                                                       | TriggerAndCollisionMask.OnTriggerStay
                                                                       | TriggerAndCollisionMask.OnTriggerExit2D
                                                                       | TriggerAndCollisionMask.OnTriggerStay2D;

        [FormerlySerializedAs("TargetLayerMask")] [Tooltip("the layers that will be affected by this tile")]
        public LayerMask targetLayerMask;

        [FormerlySerializedAs("TriggerFilter")] [Tooltip("Defines on what triggers the effect should be applied")]
        public TriggerAndCollisionMask triggerFilter = AllowedTriggerCallbacks;

        public float blizzardSpeedMultiplier = 0.5f; // Multiplier for movement speed in the Blizzard zone
        private CharacterRun _characterRun;
        private bool _isInBlizzard; // Tracks if the player is in the Blizzard zone

        private CharacterMovement _playerMovement;
        private bool _speedModified; // Tracks if speed has been modified

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (0 == (triggerFilter & TriggerAndCollisionMask.OnTriggerExit2D)) return;
            if ((targetLayerMask.value & (1 << collision.gameObject.layer)) == 0) return;

            _isInBlizzard = false;
            ApplyBlizzardEffect(collision.gameObject);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (0 == (triggerFilter & TriggerAndCollisionMask.OnTriggerStay2D)) return;
            if ((targetLayerMask.value & (1 << collision.gameObject.layer)) == 0) return;

            _isInBlizzard = true;
            ApplyBlizzardEffect(collision.gameObject);
        }

        protected virtual void ApplyBlizzardEffect(GameObject collision)
        {
            _playerMovement = collision.GetComponent<CharacterMovement>();
            _characterRun = collision.GetComponent<CharacterRun>();

            if (_playerMovement != null && _characterRun != null)
            {
                if (_isInBlizzard && !_speedModified)
                {
                    _characterRun.RunStop();
                    _characterRun.AbilityPermitted = false; // Verhindert Sprinten im Blizzard
                    _playerMovement.EnterBlizzardZone(blizzardSpeedMultiplier); // Reduziert Geschwindigkeit
                    _speedModified = true;
                }
                else if (!_isInBlizzard && _speedModified)
                {
                    _playerMovement.ExitBlizzardZone(); // Setzt normale Geschwindigkeit wieder
                    _characterRun.AbilityPermitted = true; // Sprinten wieder erlauben
                    _speedModified = false;
                }
            }
        }
    }
}