using UnityEngine;

namespace dungeonduell
{
    public class AimHandler : MonoBehaviour
    {
        [Header("Setup")]
        public Transform crosshair;               // Direkt im Inspector zuweisen
        public float aimDistance = 2f;

        private Transform _weaponTransform;
        private Vector3 _aimDirection;

        private void Awake()
        {
            _weaponTransform = transform;

            if (crosshair == null)
            {
                Debug.LogWarning("[AimHandler] Kein Crosshair zugewiesen!");
            }
            else
            {
                crosshair.gameObject.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            UpdateAimDirection();
            UpdateCrosshairPosition();
        }

        private void UpdateAimDirection()
        {
            _aimDirection = _weaponTransform.right; // Für 2D-TopDown (seitlich gerichtete Waffen)
        }

        private void UpdateCrosshairPosition()
        {
            if (crosshair == null || _aimDirection.sqrMagnitude < 0.01f)
            {
                if (crosshair != null && crosshair.gameObject.activeSelf)
                {
                    crosshair.gameObject.SetActive(false);
                }
                return;
            }

            Vector3 targetPos = _weaponTransform.position + _aimDirection.normalized * aimDistance;
            targetPos.z = 0f; // Für 2D-Spiele: Crosshair bleibt auf gleicher Z-Ebene

            crosshair.position = targetPos;

            if (!crosshair.gameObject.activeSelf)
            {
                crosshair.gameObject.SetActive(true);
            }
        }
    }
}