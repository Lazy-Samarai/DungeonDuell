using UnityEngine;
using MoreMountains.TopDownEngine;

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

        public float modifiedSpeed = 2f;        // Angepasste Bewegungsgeschwindigkeit
        public float modifiedSprintSpeed = 3f;  // Angepasste Sprintgeschwindigkeit

        private float originalSpeed;            // Originale Bewegungsgeschwindigkeit
        private float originalSprintSpeed;      // Originale Sprintgeschwindigkeit

        private bool speedModified = false;     // Flag, ob die Geschwindigkeit geändert wurde
        public bool isInBlizzard;               // Status, ob der Spieler sich im Blizzard befindet

        // Trigger-Enter Methode: Spieler bleibt auf dem Tile
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerStay2D)) return;

            // Überprüfen, ob das getroffene Objekt im richtigen Layer ist
            if ((TargetLayerMask.value & (1 << collision.gameObject.layer)) == 0) return;

            isInBlizzard = true; // Spieler ist im Blizzard
            ApplyBlizzardEffect(collision.gameObject);
        }

        // Trigger-Exit Methode: Spieler verlässt das Tile
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerExit2D)) return;

            // Überprüfen, ob das getroffene Objekt im richtigen Layer ist
            if ((TargetLayerMask.value & (1 << collision.gameObject.layer)) == 0) return;

            isInBlizzard = false; // Spieler ist nicht mehr im Blizzard
            ApplyBlizzardEffect(collision.gameObject);
        }

        protected virtual void ApplyBlizzardEffect(GameObject collision)
        {
            // Zugriff auf das CharacterMovement Skript des Spielers
            CharacterMovement playerMovement = collision.GetComponent<CharacterMovement>();
            CharacterRun characterRun = collision.GetComponent<CharacterRun>();
            
            if (playerMovement != null)
            {
                if (isInBlizzard && !speedModified)
                {
                    // Speichere die aktuelle Bewegungsgeschwindigkeit und Sprintgeschwindigkeit
                    originalSpeed = playerMovement.MovementSpeed;
                    originalSprintSpeed = characterRun.RunSpeed;

                    // Setze die Bewegungsgeschwindigkeit und Sprintgeschwindigkeit auf die modifizierten Werte
                    playerMovement.MovementSpeed = modifiedSpeed;
                    characterRun.RunSpeed = modifiedSprintSpeed;
                    speedModified = true; // Geschwindigkeiten wurden angepasst
                }
                else if (!isInBlizzard && speedModified)
                {
                    // Setze die Bewegungsgeschwindigkeit und Sprintgeschwindigkeit auf die Originalwerte zurück
                    playerMovement.MovementSpeed = originalSpeed;
                    characterRun.RunSpeed = originalSprintSpeed;
                    speedModified = false; // Geschwindigkeiten wurden zurückgesetzt
                }
            }
        }
    }
}
