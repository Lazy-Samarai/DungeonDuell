using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

namespace dungeonduell
{
    public class LevelUpPanel : MonoBehaviour
    {
        private const int AmountPerUpgrade = 1;
        [FormerlySerializedAs("LevelUpMenu")] public GameObject levelUpMenu; // Das Panel für das Level-Up
        [FormerlySerializedAs("TestHub")] public TestHub testHub;
        public bool player1;
        private DungeonDuellMultiplayerLevelManager _levelManager;
        [Header("UI Elements")] private bool _menuOpen;

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
        }

        private void OnOptionSelected(LevelUpOptions option)
        {
            if (testHub.canLevelUp & _menuOpen)
            {
                if (_levelManager != null)
                    _levelManager.ApplyLevelUpPerCoins(option, AmountPerUpgrade, player1 ? 1 : 2);

                testHub.menuShowing = false;
                ShowLevelUpMenu(false);
            }
        }
    }
}