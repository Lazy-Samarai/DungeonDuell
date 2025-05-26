using System;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace dungeonduell
{
    [Serializable]
    public class PlayerData
    {
        [FormerlySerializedAs("PlayerID")] public string playerID;
        [FormerlySerializedAs("Points")] public int points;
        [FormerlySerializedAs("Level")] public int level;

        [FormerlySerializedAs("CoinsForNextLevel")]
        public int coinsForNextLevel;

        [FormerlySerializedAs("WalkSpeed")] public float walkSpeed;
        [FormerlySerializedAs("RunSpeed")] public float runSpeed;
        [FormerlySerializedAs("MaxHealth")] public float maxHealth;
        [FormerlySerializedAs("AttackSpeed")] public float attackSpeed;

        [FormerlySerializedAs("RemainingLive")]
        public int remainingLive = 2;

        [FormerlySerializedAs("MetaHp")] public int metaHp = 100;

        [FormerlySerializedAs("MaxMetaHp")] public int maxMetaHp = 100;

        [FormerlySerializedAs("CurrentMask")] public Inventory inventory;

        public PlayerData(string playerID, int points, int level, int coinsForNextLevel, float walkSpeed,
            float runSpeed, float health, float attackSpeed, int maxMetaHp, Inventory inventory)
        {
            this.playerID = playerID;
            this.points = points;
            this.level = level;
            this.coinsForNextLevel = coinsForNextLevel;

            this.walkSpeed = walkSpeed;
            this.runSpeed = runSpeed;
            maxHealth = health;
            this.attackSpeed = attackSpeed;
            this.maxMetaHp = maxMetaHp;
            metaHp = this.maxMetaHp;
            this.inventory = inventory;
        }
    }

    public class PlayerDataManager : MonoBehaviour, IObserver
    {
        [FormerlySerializedAs("PlayerDataList")]
        public List<PlayerData> playerDataList = new();

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
            DdCodeEventHandler.DungeonConnected += FinalRound;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.DungeonConnected -= FinalRound;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void FinalRound()
        {
            nextRoundFinal = true;
            for (var i = 0; i < playerDataList.Count; i++) playerDataList[i].maxHealth = playerDataList[i].metaHp;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 1)
            {
                roundCounter++;
                if (nextRoundFinal)
                {
                    DdCodeEventHandler.Trigger_FinalRoundInDungeon();

                    SequenceMang sequenceMang;
                    if (sequenceMang = FindAnyObjectByType<SequenceMang>()) sequenceMang.DisableTimer();
                }
            }

            DdCodeEventHandler.Trigger_PlayerDataExposed(playerDataList, roundCounter);
        }
    }
}