using System;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;
using FMODUnity;
using Unity.VisualScripting;
using System.Collections;

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

        [Header("Upgrade Feedback Targets")] [SerializeField]
        private UpgradeFeedback speedFeedback;

        [SerializeField] private UpgradeFeedback attackSpeedFeedback;
        [SerializeField] private UpgradeFeedback healFeedback;

        private bool _levelUpDelay = false;

        private void Start()
        {
            _levelManager = FindFirstObjectByType<DungeonDuellMultiplayerLevelManager>();
        }

        public void UpgradeAttackSpeed()
        {
            OnOptionSelected(LevelUpOptions.AttackSpeed);
            // Hier soll das UpgradeFeedback hin
            if (attackSpeedFeedback != null)
            {
                attackSpeedFeedback.PlayFeedback();
            }
        }

        public void UpgradeSpeedd()
        {
            OnOptionSelected(LevelUpOptions.Speed);
            // Hier soll das UpgradeFeedback hin
            if (speedFeedback != null)
            {
                speedFeedback.PlayFeedback();
            }
        }

        public void UpgradeHealth()
        {
            OnOptionSelected(LevelUpOptions.HealingInstead);
            // Hier soll das UpgradeFeedback hin
            if (healFeedback != null)
            {
                healFeedback.PlayFeedback();
            }
        }

        public void ShowLevelUpMenu(bool on)
        {
            _menuOpen = on;
            levelUpMenu.SetActive(on);

            if (on)
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
            if (testHub.canLevelUp && _menuOpen)
            {
                // (Optional: Hier könnte man den EventSystem-Ansatz ganz entfernen.)

                if (_levelManager != null)
                {
                    if (!_levelUpDelay)
                    {
                        _levelManager.ApplyLevelUpPerCoins(option, AmountPerUpgrade, player1 ? 1 : 2);
                    }
                }

                RuntimeManager.PlayOneShot(optionSelectedEvent);
                testHub.menuShowing = false;
                StartCoroutine(CloseMenuWithDelay(0.5f));
            }
        }

        private IEnumerator CloseMenuWithDelay(float delay)
        {
            _levelUpDelay = true;
            yield return new WaitForSeconds(delay);
            _levelUpDelay = false;
            ShowLevelUpMenu(false);
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