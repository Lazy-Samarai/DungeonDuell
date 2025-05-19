using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Unity.Mathematics.Geometry;
using UnityEngine;

namespace dungeonduell
{
    public class EnemyLevelApply : MonoBehaviour, IObserver
    {
        public float healthLevelMuti = 1.1f;

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToAllEvents();
        }

        public void SubscribeToEvents()
        {
            DdCodeEventHandler.PlayerDataExposed += ApplyEnemyLevel;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.PlayerDataExposed -= ApplyEnemyLevel;
        }

        private void ApplyEnemyLevel(List<PlayerData> playerData, int round)
        {
            print("ApplyLevelUppy");
            Health enemyHealth = GetComponent<Health>();
            enemyHealth.MaximumHealth = Mathf.Floor(enemyHealth.MaximumHealth * Mathf.Pow(healthLevelMuti, round));
            enemyHealth.SetHealth(enemyHealth.MaximumHealth);
        }
    }
}