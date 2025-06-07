using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    [AddComponentMenu("TopDown Engine/Weapons/Weapon Laser Sight")]
    public class WeaponLaserSight : TopDownMonoBehaviour
    {
        public enum Modes { TwoD, ThreeD }

        [Header("General Settings")]
        public Modes Mode = Modes.ThreeD;
        public bool PerformRaycast = true;
        [MMCondition("PerformRaycast")]
        public bool DrawLaser = true;

        [Header("Raycast Settings")]
        public Vector3 RaycastOriginOffset;
        public Vector3 LaserOriginOffset;
        public float LaserMaxDistance = 50;
        public LayerMask LaserCollisionMask;

        [Header("Laser Settings")]
        public Vector2 LaserWidth = new Vector2(0.05f, 0.05f);
        public Material DefaultLaserMaterial; // Grün
        public Material TargetLaserMaterial;  // Rot

        [Header("Player Settings")]
        public bool IsPlayer1 = true;

        public virtual LineRenderer _line { get; protected set; }
        public virtual RaycastHit _hit { get; protected set; }
        public RaycastHit2D _hit2D;
        public virtual Vector3 _origin { get; protected set; }
        public virtual Vector3 _raycastOrigin { get; protected set; }

        protected Vector3 _destination;
        protected Vector3 _laserOffset;
        protected Weapon _weapon;
        protected Vector3 _direction;

        protected Vector3 _weaponPosition, _thisPosition, _thisForward;
        protected Quaternion _weaponRotation, _thisRotation;
        protected int _initFrame;

        protected virtual void Start()
        {
            Initialization();
        }

        protected virtual void Initialization()
        {
            if (DrawLaser)
            {
                _line = gameObject.AddComponent<LineRenderer>();
                _line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                _line.receiveShadows = false;
                _line.startWidth = LaserWidth.x;
                _line.endWidth = LaserWidth.y;
                _line.useWorldSpace = true;
                _line.material = DefaultLaserMaterial;
                _line.SetPosition(0, this.transform.position);
                _line.SetPosition(1, this.transform.position);
            }

            _weapon = GetComponent<Weapon>();
            if (_weapon == null)
            {
                Debug.LogWarning("This WeaponLaserSight is not associated to a weapon. Please add it to a GameObject with a Weapon component.");
            }

            _initFrame = Time.frameCount;
        }

        protected virtual void LateUpdate()
        {
            ShootLaser();
        }

        public virtual void ShootLaser()
        {
            if (!PerformRaycast)
            {
                return;
            }

            _laserOffset = LaserOriginOffset;
            _weaponPosition = _weapon.transform.position;
            _weaponRotation = _weapon.transform.rotation;
            _thisPosition = this.transform.position;
            _thisRotation = this.transform.rotation;
            _thisForward = this.transform.forward;

            bool targetHit = false;

            if (Mode == Modes.ThreeD)
            {
                _origin = MMMaths.RotatePointAroundPivot(_thisPosition + _laserOffset, _thisPosition, _thisRotation);
                _raycastOrigin = MMMaths.RotatePointAroundPivot(_thisPosition + RaycastOriginOffset, _thisPosition, _thisRotation);

                _hit = MMDebug.Raycast3D(_raycastOrigin, _thisForward, LaserMaxDistance, LaserCollisionMask, Color.red, true);

                if (_hit.transform != null)
                {
                    _destination = _hit.point;

                    if (_hit.collider != null && _hit.collider.transform.root != transform.root)
                    {
                        string targetTag = _hit.collider.tag;
                        if ((IsPlayer1 && targetTag == "Player2") ||
                            (!IsPlayer1 && targetTag == "Player1") ||
                            targetTag == "Enemy")
                        {
                            targetHit = true;
                        }
                    }
                }
                else
                {
                    _destination = _origin + _thisForward * LaserMaxDistance;
                }
            }
            else // 2D
            {
                _direction = _weapon.Flipped ? Vector3.left : Vector3.right;

                if (_direction == Vector3.left)
                {
                    _laserOffset.x = -LaserOriginOffset.x;
                }

                _raycastOrigin = MMMaths.RotatePointAroundPivot(_weaponPosition + _laserOffset, _weaponPosition, _weaponRotation);
                _origin = _raycastOrigin;

                _hit2D = MMDebug.RayCast(_raycastOrigin, _weaponRotation * _direction, LaserMaxDistance, LaserCollisionMask, Color.red, true);

                if (_hit2D)
                {
                    _destination = _hit2D.point;

                    if (_hit2D.collider != null && _hit2D.collider.transform.root != transform.root)
                    {
                        string targetTag = _hit2D.collider.tag;
                        if ((IsPlayer1 && targetTag == "Player2") ||
                            (!IsPlayer1 && targetTag == "Player1") ||
                            targetTag == "Enemy")
                        {
                            targetHit = true;
                        }
                    }
                }
                else
                {
                    _destination = _origin;
                    _destination.x += LaserMaxDistance * _direction.x;
                    _destination = MMMaths.RotatePointAroundPivot(_destination, _weaponPosition, _weaponRotation);
                }
            }

            // Material setzen
            _line.material = targetHit ? TargetLaserMaterial : DefaultLaserMaterial;

            if (Time.frameCount <= _initFrame + 1)
            {
                return;
            }

            if (DrawLaser)
            {
                _line.SetPosition(0, _origin);
                _line.SetPosition(1, _destination);
            }
        }

        public virtual void LaserActive(bool status)
        {
            _line.enabled = status;
        }
    }
}
