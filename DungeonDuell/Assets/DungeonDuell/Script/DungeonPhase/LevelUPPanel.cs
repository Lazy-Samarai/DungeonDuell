using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
using UnityEngine.EventSystems;
using MoreMountains.TopDownEngine;

namespace dungeonduell
{
    public class LevelUPPanel : MonoBehaviour
    {
        [Header("UI Elements")] bool menuOpen = false;
        public GameObject LevelUpMenu; // Das Panel für das Level-Up
        public TestHub TestHub;
        private DungeonDuellMultiplayerLevelManager _levelManager;
        const int AmountPerUpgrade = 1;
        public bool player1 = false;

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
            menuOpen = on;
            LevelUpMenu.SetActive(on);
        }

        private void OnOptionSelected(LevelUpOptions option)
        {
            if (TestHub.canLevelUp & menuOpen)
            {
                if (_levelManager != null)
                {
                    _levelManager.ApplyLevelUpPerCoins(option, AmountPerUpgrade, player1 ? 1 : 2);
                }

                TestHub.menuShowing = false;
                ShowLevelUpMenu(false);
            }
        }
    }
}