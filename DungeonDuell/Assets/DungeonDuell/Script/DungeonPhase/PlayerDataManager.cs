using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine.SceneManagement;


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

        public int RemainingLive = 2;


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

    public class PlayerDataManager : MonoBehaviour, IObserver
    {

        public List<PlayerData> PlayerDataList = new List<PlayerData>();

        public bool nextRoundFinal = false;

        void Awake()
        {
            PlayerDataManager[] objs = FindObjectsOfType<PlayerDataManager>();

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(gameObject);
            InitializePlayerData();
        }

        private void InitializePlayerData()
        {
            PlayerDataList.Add(new PlayerData("Player1", 0, 1, 1, 6, 10, 30, 1));
            PlayerDataList.Add(new PlayerData("Player2", 0, 1, 1, 6, 10, 30, 1));

        }

        public void FinalRound()
        {
            nextRoundFinal = true;
            for (int i = 0; i < PlayerDataList.Count; i++)
            {
                PlayerDataList[i].RemainingLive = 1;
            }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 1)
            {
                if (nextRoundFinal)
                {
                    DDCodeEventHandler.Trigger_FinalRoundInDungeon();
                    
                    SequenceMang sequenceMang;
                    if (sequenceMang = FindAnyObjectByType<SequenceMang>())
                    {
                        sequenceMang.DisableTimer();
                    }
                }
            }

        }
        void OnEnable()
        {
            SubscribeToEvents();
        }
        void OnDisable()
        {
            UnsubscribeToAllEvents();
        }
        public void SubscribeToEvents()
        {
            DDCodeEventHandler.DungeonConnected += FinalRound;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void UnsubscribeToAllEvents()
        {
            DDCodeEventHandler.DungeonConnected -= FinalRound;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

    }
}