using System;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;
using FMODUnity;
using Unity.VisualScripting;

namespace dungeonduell
{
    public class LevelUpPanel : MonoBehaviour, IObserver
    {
        private const int AmountPerUpgrade = 1;
        [FormerlySerializedAs("LevelUpMenu")] public GameObject levelUpMenu; // Das Panel für das Level-Up
        [FormerlySerializedAs("TestHub")] public TestHub testHub;
        public bool player1;
        private DungeonDuellMultiplayerLevelManager _levelManager;
        [Header("UI Elements")] private bool _menuOpen;

        [SerializeField] private EventReference openLevelUPEvent;
        [SerializeField] private EventReference closeLevelUpEvent;
        [SerializeField] private EventReference optionSelectedEvent;

        private void Start()
        {
            _levelManager = FindFirstObjectByType<DungeonDuellMultiplayerLevelManager>();
        }

        public void UpgradeAttackSpeed()
        {
            OnOptionSelected(LevelUpOptions.AttackSpeed);
        }

        public void UpgradeSpeedd()
        {
            OnOptionSelected(LevelUpOptions.Speed);
        }

        public void UpgradeHealth()
        {
            OnOptionSelected(LevelUpOptions.HealingInstead);
        }

        public void ShowLevelUpMenu(bool on)
        {
            _menuOpen = on;
            levelUpMenu.SetActive(on);

            if (on == true)
            {
                RuntimeManager.PlayOneShot(openLevelUPEvent);
            }
            else
            {
                RuntimeManager.PlayOneShot(closeLevelUpEvent);
            }
        }

        private void OnOptionSelected(LevelUpOptions option)
        {
            if (testHub.canLevelUp & _menuOpen)
            {
                if (_levelManager != null)
                    _levelManager.ApplyLevelUpPerCoins(option, AmountPerUpgrade, player1 ? 1 : 2);

                RuntimeManager.PlayOneShot(optionSelectedEvent);
                testHub.menuShowing = false;
                ShowLevelUpMenu(false);
            }
        }

        void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToAllEvents();
        }

        public void SubscribeToEvents()
        {
            DdCodeEventHandler.PlayerGotDamaged += DamageClose;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.PlayerGotDamaged -= DamageClose;
        }

        void DamageClose(bool isPlayer1)
        {
            if (_menuOpen && (player1 == isPlayer1))
            {
                ShowLevelUpMenu(false);
            }
        }
    }
}