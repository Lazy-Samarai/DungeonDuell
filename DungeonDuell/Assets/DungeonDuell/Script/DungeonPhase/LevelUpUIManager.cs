using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine.EventSystems;


namespace MoreMountains.TopDownEngine
{
    public class LevelUpUIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject LevelUpMenu; // Das Panel für das Level-Up
        public Button SpeedButton;
        public Button HealthButton;
        public Button AttackSpeedButton;

        private string _currentPlayerID;
        private DungeonDuellMultiplayerLevelManager _levelManager;

        // Setup der Buttons und Registrierung der Klick-Events
        private void Start()
        {
            _levelManager = FindObjectOfType<DungeonDuellMultiplayerLevelManager>();

            SpeedButton.onClick.AddListener(() => OnOptionSelected(LevelUpOptions.Speed));
            HealthButton.onClick.AddListener(() => OnOptionSelected(LevelUpOptions.Health));
            AttackSpeedButton.onClick.AddListener(() => OnOptionSelected(LevelUpOptions.AttackSpeed));

            HideLevelUpMenu();
        }

        public void ShowLevelUpMenu(string playerID)
        {
            _currentPlayerID = playerID;
            LevelUpMenu.SetActive(true);
        }

        public void HideLevelUpMenu()
        {
            LevelUpMenu.SetActive(false);
        }

        private void OnOptionSelected(LevelUpOptions option)
        {
            if (_levelManager != null)
            {
                _levelManager.ApplyLevelUp(option);
            }
            HideLevelUpMenu();
        }
    }
}
