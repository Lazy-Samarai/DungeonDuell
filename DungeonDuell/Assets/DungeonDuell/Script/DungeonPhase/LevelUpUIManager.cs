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


        // Tastenzuweisungen
        [Header("Key Bindings")]
        public KeyCode SpeedKey = KeyCode.Alpha1;
        public KeyCode HealthKey = KeyCode.Alpha2;
        public KeyCode AttackSpeedKey = KeyCode.Alpha3;


        // Setup der Buttons und Registrierung der Klick-Events
        private void Start()
        {
            _levelManager = FindObjectOfType<DungeonDuellMultiplayerLevelManager>();

            SpeedButton.onClick.AddListener(() => OnOptionSelected(LevelUpOptions.Speed));
            HealthButton.onClick.AddListener(() => OnOptionSelected(LevelUpOptions.Health));
            AttackSpeedButton.onClick.AddListener(() => OnOptionSelected(LevelUpOptions.AttackSpeed));

            HideLevelUpMenu();
        }

        private void Update()
        {
            // Überprüfen, ob das Menü aktiv ist
            if (LevelUpMenu.activeSelf)
            {
                // Speed-Button aktivieren
                if (Input.GetKeyDown(SpeedKey))
                {
                    Debug.Log("Speed Button aktiviert");
                    SpeedButton.onClick.Invoke();
                }

                // Health-Button aktivieren
                if (Input.GetKeyDown(HealthKey))
                {
                    Debug.Log("Health Button aktiviert");
                    HealthButton.onClick.Invoke();
                }

                // AttackSpeed-Button aktivieren
                if (Input.GetKeyDown(AttackSpeedKey))
                {
                    Debug.Log("Attack Speed Button aktiviert");
                    AttackSpeedButton.onClick.Invoke();
                }
            }
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
