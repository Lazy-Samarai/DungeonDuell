using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dungeonduell
{
    [Serializable]
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

        public int MaxMetaHp = 100;

        public MaskBase CurrentMask;

        public PlayerData(string playerID, int points, int level, int coinsForNextLevel, float walkSpeed,
            float runSpeed, float health, float attackSpeed, int maxMetaHp, MaskBase currentMask)
        {
            PlayerID = playerID;
            Points = points;
            Level = level;
            CoinsForNextLevel = coinsForNextLevel;

            WalkSpeed = walkSpeed;
            RunSpeed = runSpeed;
            MaxHealth = health;
            AttackSpeed = attackSpeed;
            MaxMetaHp = maxMetaHp;
            MetaHp = MaxMetaHp;
            CurrentMask = currentMask;
        }
    }

    public class PlayerDataManager : MonoBehaviour, IObserver
    {
        public List<PlayerData> PlayerDataList = new();

        public int roundCounter;
        public bool nextRoundFinal;

        private void Awake()
        {
            var objs = FindObjectsByType<PlayerDataManager>(FindObjectsSortMode.None);

            if (objs.Length > 1) Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
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

        public void FinalRound()
        {
            nextRoundFinal = true;
            for (var i = 0; i < PlayerDataList.Count; i++) PlayerDataList[i].MaxHealth = PlayerDataList[i].MetaHp;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 1)
            {
                roundCounter++;
                if (nextRoundFinal)
                {
                    DDCodeEventHandler.Trigger_FinalRoundInDungeon();

                    SequenceMang sequenceMang;
                    if (sequenceMang = FindAnyObjectByType<SequenceMang>()) sequenceMang.DisableTimer();
                }
            }

            DDCodeEventHandler.Trigger_PlayerDataExposed(PlayerDataList, roundCounter);
        }
    }
}