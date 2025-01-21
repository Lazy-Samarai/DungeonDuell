using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

namespace dungeonduell
{

    [System.Serializable]
    public class PlayerData
    {
        public string PlayerID;
        public int Points;
        public int Level;
        public int CoinsForNextLevel;

        public float WalkSpeed;
        public float RunSpeed;
        public float Health;
        public float AttackSpeed;

        public PlayerData(string playerID, int points, int level, int coinsForNextLevel, float walkSpeed, float runSpeed, float health, float attackSpeed)
        {
            PlayerID = playerID;
            Points = points;
            Level = level;
            CoinsForNextLevel = coinsForNextLevel;

            WalkSpeed = walkSpeed;
            RunSpeed = runSpeed;
            Health = health;
            AttackSpeed = attackSpeed;
        }
    }

    public class PlayerDataManager : MonoBehaviour, MMEventListener<SavePlayerDataEvent>, MMEventListener<LoadPlayerDataEvent>
    {
        public static PlayerDataManager Instance;

        public List<PlayerData> PlayerDataList = new List<PlayerData>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializePlayerData();
        }

        private void InitializePlayerData()
        {
            PlayerPrefs.DeleteAll();
            PlayerDataList.Add(new PlayerData("Player1", 0, 1, 1, 6, 10, 30, 1));
            PlayerDataList.Add(new PlayerData("Player2", 0, 1, 1, 6, 10, 30, 1));
        }

        public void OnMMEvent(SavePlayerDataEvent e)
        {
            foreach (var data in PlayerDataList)
            {
                PlayerPrefs.SetInt($"{data.PlayerID}_Points", data.Points);
                PlayerPrefs.SetInt($"{data.PlayerID}_Level", data.Level);
                PlayerPrefs.SetInt($"{data.PlayerID}_CoinsForNextLevel", data.CoinsForNextLevel);

                PlayerPrefs.SetFloat($"{data.PlayerID}_WalkSpeed", data.WalkSpeed);
                PlayerPrefs.SetFloat($"{data.PlayerID}_RunSpeed", data.RunSpeed);
                PlayerPrefs.SetFloat($"{data.PlayerID}_Health", data.Health);
                PlayerPrefs.SetFloat($"{data.PlayerID}_AttackSpeed", data.AttackSpeed);
            }
            PlayerPrefs.Save();
            Debug.Log("Spielerdaten gespeichert (Event).");
        }

        public void OnMMEvent(LoadPlayerDataEvent e)
        {
            foreach (var data in PlayerDataList)
            {
                data.Points = PlayerPrefs.GetInt($"{data.PlayerID}_Points", 0);
                data.Level = PlayerPrefs.GetInt($"{data.PlayerID}_Level", 1);
                data.CoinsForNextLevel = PlayerPrefs.GetInt($"{data.PlayerID}_CoinsForNextLevel", 1);

                data.WalkSpeed = PlayerPrefs.GetFloat($"{data.PlayerID}_WalkSpeed", 5.0f);  // Standardwerte anpassen
                data.RunSpeed = PlayerPrefs.GetFloat($"{data.PlayerID}_RunSpeed", 7.0f);
                data.Health = PlayerPrefs.GetFloat($"{data.PlayerID}_Health", 100.0f);
                data.AttackSpeed = PlayerPrefs.GetFloat($"{data.PlayerID}_AttackSpeed", 1.0f);
            }
            Debug.Log("Spielerdaten geladen (Event).");
        }

        private void OnEnable()
        {
            this.MMEventStartListening<SavePlayerDataEvent>();
            this.MMEventStartListening<LoadPlayerDataEvent>();
        }

        private void OnDisable()
        {
            this.MMEventStopListening<SavePlayerDataEvent>();
            this.MMEventStopListening<LoadPlayerDataEvent>();
        }
    }
}