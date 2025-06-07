using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using dungeonduell;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace MoreMountains.TopDownEngine
{
    public struct CoinEvent
    {
        public GameObject Picker;
        public int PointsToAdd;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MoreMountains.TopDownEngine.PickableItemEvent" /> struct.
        /// </summary>
        /// <param name="pickedItem">Picked item.</param>
        public CoinEvent(int pointsToAdd, GameObject picker)
        {
            Picker = picker;
            PointsToAdd = pointsToAdd;
        }

        private static CoinEvent _c;

        public static void Trigger(int pointsToAdd, GameObject picker)
        {
            _c.Picker = picker;
            _c.PointsToAdd = pointsToAdd;
            MMEventManager.TriggerEvent(_c);
        }
    }

    public enum LevelUpOptions
    {
        Speed,
        HealingInstead,
        AttackSpeed
    }

    // Partically Copy from Grasslands 
    public class DungeonDuellMultiplayerLevelManager : MultiplayerLevelManager, MMEventListener<CoinEvent>, IObserver
    {
        private const string PlayerNamebase = "Player";

        private const int HealingPreFinal = 40;
        private const int HealingInFinal = 80;

        /// the list of countdowns we need to update
        [FormerlySerializedAs("Countdowns")] [Tooltip("the list of countdowns we need to update")]
        public List<MMCountdown> countdowns;

        public CharacterMovement[] walking = new CharacterMovement[2];
        public CharacterRun[] running = new CharacterRun[2];
        public ProjectileWeapon[] weapon = new ProjectileWeapon[2];
        public Health[] health = new Health[2];

        public PlayerSpineAnimationHandling[] playerSpineAnimationHandlings = new PlayerSpineAnimationHandling[2];

        public SequenceMang sequenceMang;

        [SerializeField] private float coastMultiply = 2;
        [SerializeField] private int startCoast = 1;

        private int _healthOnUpgrade = HealingPreFinal;

        protected string PlayerID;

        private PlayerDataManager _playerDataManager;


        [Header("Bindings")]
        // An array to store each player's points 
        [Tooltip("an array to store each player's points")]
        public DdPoints[] Points;

        public virtual string WinnerID { get; set; }
        public virtual string LevelUpid { get; set; }


        /// <summary>
        ///     Starts listening for pickable item events
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            this.MMEventStartListening<CoinEvent>();
            SubscribeToEvents();
        }

        /// <summary>
        ///     Stops listening for pickable item events
        /// </summary>
        protected override void OnDisable()
        {
            SavePlayerStates();
            base.OnDisable();
            this.MMEventStopListening<CoinEvent>();
            UnsubscribeToAllEvents();
        }

        public void SubscribeToEvents()
        {
            DdCodeEventHandler.PlayerUpgrade += HandleUpgrade;
            DdCodeEventHandler.PlayerUpgradeWithMask += HandleUpgrade;
            DdCodeEventHandler.FinalRoundInDungeon += HealingIncreased;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.PlayerUpgrade -= HandleUpgrade;
            DdCodeEventHandler.PlayerUpgradeWithMask -= HandleUpgrade;
            DdCodeEventHandler.FinalRoundInDungeon -= HealingIncreased;
        }

        /// <summary>
        ///     When a coin gets picked, we increase the amount of points of the character who picked it
        /// </summary>
        /// <param name="pickEvent"></param>
        public virtual void OnMMEvent(CoinEvent coinEvent)
        {
            LevelUpid = coinEvent.Picker.MMGetComponentNoAlloc<Character>()?.PlayerID;
            for (var i = 0; i < Points.Length; i++)
                if (Points[i].PlayerID == LevelUpid)
                {
                    Points[i].Points += coinEvent.PointsToAdd;
                    TopDownEngineEvent.Trigger(TopDownEngineEventTypes.Repaint, null);
                    if (Points[i].Points >= Points[i].CoinsForNextLevel) HandleUpgradable(i);
                }
        }


        /// <summary>
        ///     On init, we initialize our points and countdowns
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            walking[0] = GetPlayerMovement(1);
            walking[1] = GetPlayerMovement(2);

            running[0] = GetPlayerRun(1);
            running[1] = GetPlayerRun(2);

            playerSpineAnimationHandlings[0] = GetPlayerSpineAnimationHandling(1);
            playerSpineAnimationHandlings[1] = GetPlayerSpineAnimationHandling(2);

            health[0] = GetPlayerHealth(1);
            health[1] = GetPlayerHealth(2);

            WinnerID = "";
            LevelUpid = "";

            Points = new DdPoints[Players.Count];
            var i = 0;
            foreach (var player in Players)
            {
                Points[i].PlayerID = player.PlayerID;
                Points[i].Points = 0;
                Points[i].Level = 1;
                Points[i].CoinsForNextLevel = startCoast; // Startkosten 
                i++;
            }

            _playerDataManager = FindAnyObjectByType<PlayerDataManager>();

            // Apply 
            SynchronizeFromPlayerDataManager();
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.Repaint, null);
        }

        public void SavePlayerStates()
        {
            SynchronizeToPlayerDataManager();
        }

        private void SynchronizeToPlayerDataManager()
        {
            var playerDataList = _playerDataManager.playerDataList;
            for (var i = 0; i < Points.Length; i++)
                foreach (var data in playerDataList)
                    if (data.playerID == Points[i].PlayerID)
                    {
                        data.points = Points[i].Points;
                        data.level = Points[i].Level;
                        data.coinsForNextLevel = Points[i].CoinsForNextLevel;

                        data.walkSpeed = walking[i].WalkSpeed;
                        data.runSpeed = running[i].RunSpeed;
                        data.maxHealth = health[i].MaximumHealth;
                        data.attackSpeed = weapon[i].TimeBetweenUses;

                        // Give remain Hp back to meta 
                        data.metaHp += (int)health[i].CurrentHealth;
                    }
        }

        private void SynchronizeFromPlayerDataManager()
        {
            var playerDataList = _playerDataManager.playerDataList;
            for (var i = 0; i < Points.Length; i++)
                foreach (var data in playerDataList)
                    if (data.playerID == Points[i].PlayerID)
                    {
                        Points[i].Points = data.points;
                        Points[i].Level = data.level;
                        Points[i].CoinsForNextLevel = data.coinsForNextLevel;

                        // Spielerattribute synchronisieren 
                        walking[i].WalkSpeed = data.walkSpeed;
                        running[i].RunSpeed = data.runSpeed;
                        health[i].MaximumHealth = data.maxHealth;


                        // Set Heath
                        health[i].InitialHealth = Math.Min(data.metaHp, health[i].MaximumHealth);
                        // Upate Player MetaHp
                        data.metaHp = (int)Math.Max(data.metaHp - health[i].MaximumHealth, 0);

                        MaskBase currentMask = (MaskBase)data.inventory.Content[0];

                        if (currentMask != null)
                            playerSpineAnimationHandlings[i].SetSkin(((UpgradeMask)currentMask).skinId);

                        if (Points[i].Points >= Points[i].CoinsForNextLevel) HandleUpgradable(i);
                    }
        }

        /// <summary>
        ///     Whenever a player dies, we check if we only have one left alive, in which case we trigger our game over routine
        /// </summary>
        /// <param name="playerCharacter"></param>
        protected override void OnPlayerDeath(Character playerCharacter)
        {
            var playerIndex = int.Parse(playerCharacter.PlayerID[^1].ToString()) - 1;
            base.OnPlayerDeath(playerCharacter);
            var aliveCharacters = 0;
            var i = 0;

            if (_playerDataManager.playerDataList[playerIndex].metaHp <= 0)
            {
                if (playerCharacter.PlayerID == "Player1")
                    WinnerID = "Player2";
                else
                    WinnerID = "Player1";

                StartCoroutine(GameOver());
            }


            foreach (var character in Instance.Players)
            {
                if (character.ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead)
                {
                    WinnerID = character.PlayerID;
                    aliveCharacters++;
                }

                i++;
            }

            if (aliveCharacters <= 0) sequenceMang.BackToCardPhase();
        }

        /// <summary>
        ///     On game over, displays the game over screen
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator GameOver()
        {
            if (WinnerID == "") WinnerID = "Player1";
            DdCodeEventHandler.Trigger_WeHaveWinner(WinnerID);
            yield return new WaitForSeconds(5f);
            MMSoundManagerAllSoundsControlEvent.Trigger(MMSoundManagerAllSoundsControlEventTypes.FreeAllLooping);
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.GameOver, null);
            yield return new WaitForSeconds(0.1f); // Still Press Space to Coutinue  
        }

        protected virtual void CheckForGameOver()
        {
        }

        private void HandleUpgradable(int playerID)
        {
            var upgradableCount =
                (int)Math.Floor(Math.Log(
                    1 + (coastMultiply - 1) * Points[playerID].Points / Points[playerID].CoinsForNextLevel,
                    coastMultiply));
            DdCodeEventHandler.Trigger_LevelUpAvailable(playerID, upgradableCount);
        }

        public void ApplyLevelUpPerCoins(LevelUpOptions option, int amount, int playerId)
        {
            var fullPlayerId = PlayerNamebase + playerId;
            for (var i = 0; i < Points.Length; i++)
                if (Points[i].PlayerID == fullPlayerId)
                {
                    Points[i].Points -= Points[i].CoinsForNextLevel;
                    Points[i].CoinsForNextLevel =
                        (int)(Points[i].CoinsForNextLevel * coastMultiply); // Kosten erhöhen per Mutiply 
                    Points[i].Level++;

                    HandleUpgradable(i);
                    TopDownEngineEvent.Trigger(TopDownEngineEventTypes.Repaint, null);

                    HandleUpgrade(option, fullPlayerId, amount);

                    if (Points[i].Points < Points[i].CoinsForNextLevel)
                    {
                        DdCodeEventHandler.Trigger_LevelUpAvailable(i, 0);
                        TopDownEngineEvent.Trigger(TopDownEngineEventTypes.Repaint, null);
                    }
                }
        }

        private void HandleUpgrade(LevelUpOptions option, string playerReference, int amount)
        {
            var playerIndex = int.Parse(playerReference[^1].ToString()) - 1;
            switch (option)
            {
                case LevelUpOptions.Speed:
                    UpgradeSpeed(playerIndex, amount);
                    break;
                case LevelUpOptions.AttackSpeed:
                    UpgradeWeaponSpeed(playerIndex, amount);
                    break;
                case LevelUpOptions.HealingInstead:
                    Healing(playerIndex, amount);
                    break;
            }
        }

        private void HandleUpgrade(LevelUpOptions option, string playerReference, int amount, int mask)
        {
            HandleUpgrade(option, playerReference, amount);
            playerSpineAnimationHandlings[int.Parse(playerReference[^1].ToString()) - 1].SetSkin(mask);
        }


        private void Healing(int playerID, int amount)
        {
            health[playerID].ReceiveHealth(_healthOnUpgrade * amount, health[playerID].gameObject);
        }

        private void UpgradeSpeed(int playerID, int amount)
        {
            var defaultWalking = walking[playerID].WalkSpeed / playerSpineAnimationHandlings[playerID].walkMultiply;
            var defaultRunning = running[playerID].RunSpeed / playerSpineAnimationHandlings[playerID].runningMultiply;

            walking[playerID].WalkSpeed += 1.0f * amount;
            walking[playerID].MovementSpeed += 1.0f * amount;
            running[playerID].RunSpeed += 1.0f * amount;


            playerSpineAnimationHandlings[playerID].walkMultiply = walking[playerID].WalkSpeed / defaultWalking;
            playerSpineAnimationHandlings[playerID].runningMultiply = running[playerID].RunSpeed / defaultRunning;
        }

        private void UpgradeWeaponSpeed(int playerID, int amount)
        {
            weapon[playerID].TimeBetweenUses *= (float)Math.Pow(0.85f, amount);
        }

        private Health GetPlayerHealth(int i)
        {
            foreach (var playerHealth in FindObjectsByType<Health>(FindObjectsSortMode.None))
                if (playerHealth.GetComponent<Character>().PlayerID == PlayerNamebase + i)
                    return playerHealth;

            return null;
        }

        private CharacterMovement GetPlayerMovement(int i)
        {
            foreach (var movement in FindObjectsByType<CharacterMovement>(FindObjectsSortMode.None))
                if (movement.GetComponent<Character>().PlayerID == PlayerNamebase + i)
                    return movement;

            return null;
        }

        private CharacterRun GetPlayerRun(int i)
        {
            foreach (var run in FindObjectsByType<CharacterRun>(FindObjectsSortMode.None))
                if (run.GetComponent<Character>().PlayerID == PlayerNamebase + i)
                    return run;

            return null;
        }

        private PlayerSpineAnimationHandling GetPlayerSpineAnimationHandling(int i)
        {
            foreach (var character in FindObjectsByType<Character>(FindObjectsSortMode.None))
                if (character.PlayerID == PlayerNamebase + i)
                    return character.GetComponentInChildren<PlayerSpineAnimationHandling>();

            return null;
        }


        public void RegisterAndUpdateWeapon(ProjectileWeapon weap, string id)
        {
            // Weapons is later Inhilaizes so its updates it self by calling this Function ist self
            var playerIndex = int.Parse(id[id.Length - 1].ToString()) - 1;
            weapon[playerIndex] = weap;
            weapon[playerIndex].TimeBetweenUses = _playerDataManager.playerDataList[playerIndex].attackSpeed;
        }

        private void HealingIncreased()
        {
            _healthOnUpgrade = HealingInFinal;
        }

        public struct DdPoints
        {
            public string PlayerID;
            public int Points;
            public int Level;
            public int CoinsForNextLevel;
        }
    }
}