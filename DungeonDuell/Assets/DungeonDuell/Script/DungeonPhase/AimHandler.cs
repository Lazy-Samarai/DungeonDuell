using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.TopDownEngine;

namespace dungeonduell
{
    public class AimHandler : MonoBehaviour
    {
        [Header("Setup")]
        public Transform crosshair;               // Nicht im Prefab zuweisen!
        public float aimDistance = 2f;

        private Transform _weaponTransform;
        private Vector3 _aimDirection;
        private Weapon _weapon;
        private PlayerInput _playerInput;
        private Camera _myCamera;

        private void Start()
        {
            _weaponTransform = transform;
            _weapon = GetComponent<Weapon>();
            _playerInput = GetComponentInParent<PlayerInput>();
            _myCamera = GetComponentInParent<Camera>();

            if (_playerInput == null || _myCamera == null)
            {
                Debug.LogWarning("[AimHandler] Kein PlayerInput oder Kamera gefunden!");
                return;
            }

            if (crosshair == null && _weapon != null && _weapon.Owner != null)
            {
                string playerTag = _weapon.Owner.tag; // z. B. "Player1"
                string crosshairTag = playerTag.Replace("Player", "Crosshair"); // → "Crosshair1"
                GameObject found = GameObject.FindGameObjectWithTag(crosshairTag);
                if (found != null)
                {
                    crosshair = found.transform;
                    Debug.Log("[AimHandler] Crosshair zugewiesen: " + crosshair.name);
                }
                else
                {
                    Debug.LogWarning("[AimHandler] Kein Crosshair gefunden für Tag '" + crosshairTag + "'");
                }
            }

            if (crosshair != null)
            {
                crosshair.gameObject.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            if (_playerInput == null || _myCamera == null || crosshair == null) return;

            UpdateAimDirection();
            UpdateCrosshairPosition();
        }

        private void UpdateAimDirection()
        {
            Vector2 aimInput = _playerInput.actions["Aim"].ReadValue<Vector2>();

            // Wenn mit Stick gezielt wird (z. B. Controller)
            if (aimInput.sqrMagnitude > 0.01f)
            {
                _aimDirection = new Vector3(aimInput.x, aimInput.y, 0f).normalized;
                return;
            }

            // Optional: Falls mit Maus gezielt wird (Screen Position → World)
            Vector2 screenPos = _playerInput.actions["Point"].ReadValue<Vector2>();
            Vector3 worldPos = _myCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
            worldPos.z = 0f;
            _aimDirection = (worldPos - _weaponTransform.position).normalized;
        }

        private void UpdateCrosshairPosition()
        {
            if (_aimDirection.sqrMagnitude < 0.01f)
            {
                if (crosshair.gameObject.activeSelf)
                {
                    crosshair.gameObject.SetActive(false);
                }
                return;
            }

            Vector3 targetPos = _weaponTransform.position + _aimDirection * aimDistance;
            targetPos.z = 0f;

            crosshair.position = targetPos;
            crosshair.rotation = Quaternion.identity;

            if (!crosshair.gameObject.activeSelf)
            {
                crosshair.gameObject.SetActive(true);
            }
        }
    }
}
