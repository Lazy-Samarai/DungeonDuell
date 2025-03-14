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
        public float MaxHealth;
        public float AttackSpeed;

        public int RemainingLive = 2;

        public int MetaHp = 100;

        public PlayerData(string playerID, int points, int level, int coinsForNextLevel, float walkSpeed, float runSpeed, float health, float attackSpeed,int metaHp)
        {
            PlayerID = playerID;
            Points = points;
            Level = level;
            CoinsForNextLevel = coinsForNextLevel;

            WalkSpeed = walkSpeed;
            RunSpeed = runSpeed;
            MaxHealth = health;
            AttackSpeed = attackSpeed;
            MetaHp = metaHp;
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
        }
        public void FinalRound()
        {
            nextRoundFinal = true;
            for (int i = 0; i < PlayerDataList.Count; i++)
            {
                PlayerDataList[i].MaxHealth = PlayerDataList[i].MetaHp;
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