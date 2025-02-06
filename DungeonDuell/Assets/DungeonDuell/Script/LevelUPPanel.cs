using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
using UnityEngine.EventSystems;
using MoreMountains.TopDownEngine;

namespace dungeonduell
{
    public class LevelUPPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject LevelUpMenu; // Das Panel für das Level-Up
        public TestHub TestHub;
        public Button SpeedButton;
        public Button HealthButton;
        public Button AttackSpeedButton;

        private DungeonDuellMultiplayerLevelManager _levelManager;

        // Tastenzuweisungen
        [Header("Key Bindings")]
        public KeyCode SpeedKey = KeyCode.Alpha1;
        public KeyCode HealthKey = KeyCode.Alpha2;
        public KeyCode AttackSpeedKey = KeyCode.Alpha3;
        public KeyCode LevelUpUIKey = KeyCode.Alpha4;

        bool showMenu = false;
        
        private void Start()
        {
            _levelManager = FindObjectOfType<DungeonDuellMultiplayerLevelManager>();
        }

        public void UpgradeAttackSpeed()
        {

            OnOptionSelected(LevelUpOptions.AttackSpeed);
        }
         public void UpgradeHealth()
        {
            OnOptionSelected(LevelUpOptions.Health);
        }
         public void UpgradeSpeedd()
        {
            OnOptionSelected(LevelUpOptions.Speed);
        }

        public void ShowLevelUpMenu(bool on)
        {
            LevelUpMenu.SetActive(on);
        }

        private void OnOptionSelected(LevelUpOptions option)
        {
            if (TestHub.canLevelUp)
            {
                if (_levelManager != null)
                {
                    _levelManager.ApplyLevelUp(option);
                }
                //TestHub.canLevelUp = false;
                ShowLevelUpMenu(false);
            }
            

        }
    }
}
