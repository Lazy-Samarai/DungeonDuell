using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using System;
using MoreMountains.Feedbacks;
using UnityEngine.Events;
using UnityEngine.Serialization;
using MoreMountains.TopDownEngine;

namespace dungeonduell
{
    /// <summary>
    /// Add this component to an object and it will heal objects that collide with it. 
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Heal/HealOnTouch")]
    public class HealOnTouch : MMMonoBehaviour
    {
        [Flags]
        public enum TriggerAndCollisionMask
        {
            IgnoreAll = 0,
            OnTriggerEnter = 1 << 0,
            OnTriggerStay = 1 << 1,
            OnTriggerEnter2D = 1 << 6,
            OnTriggerStay2D = 1 << 7,

            All_3D = OnTriggerEnter | OnTriggerStay,
            All_2D = OnTriggerEnter2D | OnTriggerStay2D,
            All = All_3D | All_2D
        }

        public const TriggerAndCollisionMask AllowedTriggerCallbacks = TriggerAndCollisionMask.OnTriggerEnter
                                                                      | TriggerAndCollisionMask.OnTriggerStay
                                                                      | TriggerAndCollisionMask.OnTriggerEnter2D
                                                                      | TriggerAndCollisionMask.OnTriggerStay2D;

        [MMInspectorGroup("Targets", true, 3)]
        [MMInformation(
            "This component will make your object heal other objects that collide with it. Here you can define what layers will be affected by the healing, how much health to restore, and how long the post-heal invincibility should last (in seconds).",
            MMInformationAttribute.InformationType.Info, false)]
        /// the layers that will be healed by this object
        [Tooltip("the layers that will be healed by this object")]
        public LayerMask TargetLayerMask;
        /// the owner of the HealOnTouch zone
        [MMReadOnly]
        [Tooltip("the owner of the HealOnTouch zone")]
        public GameObject Owner;

        /// Defines on what triggers the healing should be applied
        [Tooltip("Defines on what triggers the healing should be applied")]
        public TriggerAndCollisionMask TriggerFilter = AllowedTriggerCallbacks;

        [MMInspectorGroup("Healing Caused", true, 8)]
        /// The amount of health to restore
        [Tooltip("The amount of health to restore")]
        public float HealAmount;

        [Header("Invincibility")]
        /// The duration of the invincibility frames after healing (in seconds)
        [Tooltip("The duration of the invincibility frames after healing (in seconds)")]
        public float InvincibilityDuration = 0.5f;

        [MMInspectorGroup("Feedbacks", true, 18)]
        /// the feedback to play when healing a Damageable
        [Tooltip("the feedback to play when healing a Damageable")]
        public MMFeedbacks HealDamageableFeedback;

        /// an event to trigger when healing a Damageable
        public UnityEvent<Health> HealDamageableEvent;

        protected Health _colliderHealth;
        protected List<GameObject> _ignoredGameObjects;

        protected virtual void Awake()
        {
            Initialization();
        }

        protected virtual void OnEnable()
        {
            _ignoredGameObjects.Clear();
        }

        public virtual void Initialization()
        {
            InitializeIgnoreList();
        }

        protected virtual void InitializeIgnoreList()
        {
            if (_ignoredGameObjects == null) _ignoredGameObjects = new List<GameObject>();
        }

        protected virtual void OnDisable()
        {
            ClearIgnoreList();
        }

        protected virtual void OnTriggerStay2D(Collider2D collider)
        {
            if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerStay2D)) return;
            Colliding(collider.gameObject);
        }

        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerEnter2D)) return;
            Colliding(collider.gameObject);
        }

        protected virtual void Colliding(GameObject collider)
        {
            if (!EvaluateAvailability(collider))
            {
                return;
            }

            _colliderHealth = collider.gameObject.MMGetComponentNoAlloc<Health>();

            // if what we're colliding with is healable
            if (_colliderHealth != null)
            {
                if (_colliderHealth.CurrentHealth > 0)
                {
                    OnCollideWithDamageable(_colliderHealth);
                }
            }
        }

        protected virtual bool EvaluateAvailability(GameObject collider)
        {
            if (!isActiveAndEnabled) { return false; }

            if (_ignoredGameObjects.Contains(collider)) { return false; }

            if (!MMLayers.LayerInLayerMask(collider.layer, TargetLayerMask)) { return false; }

            return true;
        }

        protected virtual void OnCollideWithDamageable(Health health)
        {
            _colliderHealth = health;

            if (health.CanHealThisFrame()) // Du musst diese Methode möglicherweise hinzufügen
            {
                HealDamageableFeedback?.PlayFeedbacks(this.transform.position);
                HealDamageableEvent?.Invoke(_colliderHealth);

                // Hier wird die Heilung angewendet
                //health.ReceiveHealth(HealAmount, gameObject); // Übergebe die Menge an Heilung
                StartCoroutine(health.HealOverTime(HealAmount, gameObject, 5f, 0.25f)); // 2 Sekunden für die gesamte Heilung, 0.5 Sekunden zwischen den Schüben
            }
        }

        public virtual void IgnoreGameObject(GameObject newIgnoredGameObject)
        {
            InitializeIgnoreList();
            _ignoredGameObjects.Add(newIgnoredGameObject);
        }

        public virtual void StopIgnoringObject(GameObject ignoredGameObject)
        {
            if (_ignoredGameObjects != null) _ignoredGameObjects.Remove(ignoredGameObject);
        }

        public virtual void ClearIgnoreList()
        {
            InitializeIgnoreList();
            _ignoredGameObjects.Clear();
        }
    }
}
