using dungeonduell;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    public struct CoinEvent
    {
        public GameObject Picker;
        public int PointsToAdd;

        /// <summary> 
        /// Initializes a new instance of the <see cref="MoreMountains.TopDownEngine.PickableItemEvent"/> struct. 
        /// </summary> 
        /// <param name="pickedItem">Picked item.</param> 
        public CoinEvent(int pointsToAdd, GameObject picker)
        {
            Picker = picker;
            PointsToAdd = pointsToAdd;
        }

        static CoinEvent c;

        public static void Trigger(int pointsToAdd, GameObject picker)
        {
            c.Picker = picker;
            c.PointsToAdd = pointsToAdd;
            MMEventManager.TriggerEvent(c);
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
        public struct DDPoints
        {
            public string PlayerID;
            public int Points;
            public int Level;
            public int CoinsForNextLevel;
        }


        [Header("Bindings")]
        // An array to store each player's points 
        [Tooltip("an array to store each player's points")]
        public DDPoints[] Points;

        /// the list of countdowns we need to update 
        [Tooltip("the list of countdowns we need to update")]
        public List<MMCountdown> Countdowns;

        public virtual string WinnerID { get; set; }
        public virtual string LevelUPID { get; set; }

        protected string _playerID;
        protected bool _gameOver = false;

        public CharacterMovement[] walking = new CharacterMovement[2];
        public CharacterRun[] running = new CharacterRun[2];
        public ProjectileWeapon[] weapon = new ProjectileWeapon[2];
        public Health[] health = new Health[2];

        public PlayerSpineAnimationHandling[] playerSpineAnimationHandlings = new PlayerSpineAnimationHandling[2];

        public dungeonduell.SequenceMang sequenceMang;

        const string playerNamebase = "Player";

        PlayerDataManager playerDataManager;

        [SerializeField] float coastMultiply = 2;
        [SerializeField] int startCoast = 1;

        private int _healthOnUpgrade = HealingPreFinal;

        const int HealingPreFinal = 40;
        const int HealingInFinal = 80;


        /// <summary> 
        /// On init, we initialize our points and countdowns 
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
            LevelUPID = "";

            Points = new DDPoints[Players.Count];
            int i = 0;
            foreach (Character player in Players)
            {
                Points[i].PlayerID = player.PlayerID;
                Points[i].Points = 0;
                Points[i].Level = 1;
                Points[i].CoinsForNextLevel = startCoast; // Startkosten 
                i++;
            }

            playerDataManager = FindAnyObjectByType<PlayerDataManager>();

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
            var playerDataList = playerDataManager.PlayerDataList;
            for (int i = 0; i < Points.Length; i++)
            {
                foreach (var data in playerDataList)
                {
                    if (data.PlayerID == Points[i].PlayerID)
                    {
                        data.Points = Points[i].Points;
                        data.Level = Points[i].Level;
                        data.CoinsForNextLevel = Points[i].CoinsForNextLevel;

                        data.WalkSpeed = walking[i].WalkSpeed;
                        data.RunSpeed = running[i].RunSpeed;
                        data.MaxHealth = health[i].MaximumHealth;
                        data.AttackSpeed = weapon[i].TimeBetweenUses;


                        // Give remain Hp back to meta 
                        data.MetaHp += (int)health[i].CurrentHealth;
                    }
                }
            }
        }

        private void SynchronizeFromPlayerDataManager()
        {
            var playerDataList = playerDataManager.PlayerDataList;
            for (int i = 0; i < Points.Length; i++)
            {
                foreach (var data in playerDataList)
                {
                    if (data.PlayerID == Points[i].PlayerID)
                    {
                        Points[i].Points = data.Points;
                        Points[i].Level = data.Level;
                        Points[i].CoinsForNextLevel = data.CoinsForNextLevel;

                        // Spielerattribute synchronisieren 
                        walking[i].WalkSpeed = data.WalkSpeed;
                        running[i].RunSpeed = data.RunSpeed;
                        health[i].MaximumHealth = data.MaxHealth;


                        // Set Heath
                        health[i].InitialHealth = Math.Min(data.MetaHp, health[i].MaximumHealth);
                        // Upate Player MetaHp
                        data.MetaHp = (int)Math.Max(data.MetaHp - health[i].MaximumHealth, 0);
                    }
                }
            }
        }

        /// <summary> 
        /// Whenever a player dies, we check if we only have one left alive, in which case we trigger our game over routine 
        /// </summary> 
        /// <param name="playerCharacter"></param> 
        protected override void OnPlayerDeath(Character playerCharacter)
        {
            int playerIndex = Int32.Parse(playerCharacter.PlayerID[^1].ToString()) - 1;
            base.OnPlayerDeath(playerCharacter);
            int aliveCharacters = 0;
            int i = 0;

            if (playerDataManager.PlayerDataList[playerIndex].MetaHp <= 0)
            {
                if (playerCharacter.PlayerID == "Player1")
                {
                    WinnerID = "Player2";
                }
                else
                {
                    WinnerID = "Player1";
                }

                StartCoroutine(GameOver());
            }


            foreach (Character character in LevelManager.Instance.Players)
            {
                if (character.ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead)
                {
                    WinnerID = character.PlayerID;
                    aliveCharacters++;
                }

                i++;
            }

            if (aliveCharacters <= 0)
            {
                sequenceMang.BackToCardPhase();
            }
        }

        /// <summary> 
        /// On game over, freezes time and displays the game over screen 
        /// </summary> 
        /// <returns></returns> 
        protected virtual IEnumerator GameOver()
        {
            yield return new WaitForSeconds(2f);
            if (WinnerID == "")
            {
                WinnerID = "Player1";
            }

            MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 0f, 0f, false, 0f, true);
            _gameOver = true;
            MMSoundManagerAllSoundsControlEvent.Trigger(MMSoundManagerAllSoundsControlEventTypes.FreeAllLooping);
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.GameOver, null);
            yield return new WaitForSeconds(0.1f); // Still Press Space to Coutinue  
            sequenceMang.Reseting();
        }

        protected virtual void CheckForGameOver()
        {
            if (_gameOver)
            {
                if (Input.GetButton("Player1_Jump")
                    || Input.GetButton("Player2_Jump")
                    || Input.GetButton("Player3_Jump")
                    || Input.GetButton("Player4_Jump"))
                {
                    MMTimeScaleEvent.Trigger(MMTimeScaleMethods.Reset, 1f, 0f, false, 0f, true);
                    // MMSceneLoadingManager.LoadScene(SceneManager.GetActiveScene().name); 
                }
            }
        }

        /// <summary> 
        /// When a coin gets picked, we increase the amount of points of the character who picked it 
        /// </summary> 
        /// <param name="pickEvent"></param> 
        public virtual void OnMMEvent(CoinEvent coinEvent)
        {
            LevelUPID = coinEvent.Picker.MMGetComponentNoAlloc<Character>()?.PlayerID;
            for (int i = 0; i < Points.Length; i++)
            {
                if (Points[i].PlayerID == LevelUPID)
                {
                    Points[i].Points += coinEvent.PointsToAdd;
                    TopDownEngineEvent.Trigger(TopDownEngineEventTypes.Repaint, null);
                    if (Points[i].Points >= Points[i].CoinsForNextLevel)
                    {
                        HandleUpgradable(i);
                    }
                }
            }
        }

        private void HandleUpgradable(int playerID)
        {
            int upgradableCount =
                (int)Math.Floor(Math.Log(
                    1 + ((coastMultiply - 1) * Points[playerID].Points / Points[playerID].CoinsForNextLevel),
                    coastMultiply));
            DDCodeEventHandler.Trigger_LevelUpAvailable(playerID, upgradableCount);
        }

        public void ApplyLevelUpPerCoins(LevelUpOptions option, int amount, int playerId)
        {
            string fullPlayerId = (playerNamebase + playerId);
            for (int i = 0; i < Points.Length; i++)
            {
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
                        DDCodeEventHandler.Trigger_LevelUpAvailable(i, 0);
                        TopDownEngineEvent.Trigger(TopDownEngineEventTypes.Repaint, null);
                    }
                }
            }
        }

        private void HandleUpgrade(LevelUpOptions option, String playerReference, int amount)
        {
            int playerIndex = Int32.Parse(playerReference[^1].ToString()) - 1;
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

        private void Healing(int playerID, int amount)
        {
            health[playerID].ReceiveHealth(_healthOnUpgrade * amount, health[playerID].gameObject);
        }

        private void UpgradeSpeed(int playerID, int amount)
        {
            float defaultWalking = walking[playerID].WalkSpeed / playerSpineAnimationHandlings[playerID].walkMultiply;
            float defaultRunning = running[playerID].RunSpeed / playerSpineAnimationHandlings[playerID].runningMultiply;

            print(amount);

            walking[playerID].WalkSpeed += 1.0f * amount;
            walking[playerID].MovementSpeed += 1.0f * amount;
            running[playerID].RunSpeed += 1.0f * amount;


            playerSpineAnimationHandlings[playerID].walkMultiply = walking[playerID].WalkSpeed / defaultWalking;
            playerSpineAnimationHandlings[playerID].runningMultiply = running[playerID].RunSpeed / defaultRunning;
        }

        private void UpgradeWeaponSpeed(int playerID, int amount)
        {
            print((float)Math.Pow(0.85f, amount));
            weapon[playerID].TimeBetweenUses *= (float)Math.Pow(0.85f, amount);
        }

        private Health GetPlayerHealth(int i)
        {
            foreach (Health health in FindObjectsByType<Health>(FindObjectsSortMode.None))
            {
                if (health.GetComponent<Character>().PlayerID == (playerNamebase + i))
                {
                    return health;
                }
            }

            return null;
        }

        private CharacterMovement GetPlayerMovement(int i)
        {
            foreach (CharacterMovement movement in FindObjectsByType<CharacterMovement>(FindObjectsSortMode.None))
            {
                if (movement.GetComponent<Character>().PlayerID == (playerNamebase + i))
                {
                    return movement;
                }
            }

            return null;
        }

        private CharacterRun GetPlayerRun(int i)
        {
            foreach (CharacterRun run in FindObjectsByType<CharacterRun>(FindObjectsSortMode.None))
            {
                if (run.GetComponent<Character>().PlayerID == (playerNamebase + i))
                {
                    return run;
                }
            }

            return null;
        }

        private Character GetPlayerCharacter(int i)
        {
            foreach (Character character in FindObjectsByType<Character>(FindObjectsSortMode.None))
            {
                if (character.PlayerID == (playerNamebase + i))
                {
                    return character;
                }
            }

            return null;
        }

        private PlayerSpineAnimationHandling GetPlayerSpineAnimationHandling(int i)
        {
            foreach (Character character in FindObjectsByType<Character>(FindObjectsSortMode.None))
            {
                if (character.PlayerID == (playerNamebase + i))
                {
                    return character.GetComponentInChildren<PlayerSpineAnimationHandling>();
                }
            }

            return null;
        }


        public void RegisterAndUpdateWeapon(ProjectileWeapon weap, string id)
        {
            // Weapons is later Inhilaizes so its updates it self by calling this Function ist self
            int playerIndex = Int32.Parse(id[id.Length - 1].ToString()) - 1;
            weapon[playerIndex] = weap;
            weapon[playerIndex].TimeBetweenUses = playerDataManager.PlayerDataList[playerIndex].AttackSpeed;
        }

        private void HealingIncreased()
        {
            _healthOnUpgrade = HealingInFinal;
        }


        /// <summary> 
        /// Starts listening for pickable item events 
        /// </summary> 
        protected override void OnEnable()
        {
            base.OnEnable();
            this.MMEventStartListening<CoinEvent>();
            SubscribeToEvents();
        }

        /// <summary> 
        /// Stops listening for pickable item events 
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
            DDCodeEventHandler.PlayerUpgrade += HandleUpgrade;
            DDCodeEventHandler.FinalRoundInDungeon += HealingIncreased;
        }

        public void UnsubscribeToAllEvents()
        {
            DDCodeEventHandler.PlayerUpgrade -= HandleUpgrade;
            DDCodeEventHandler.FinalRoundInDungeon -= HealingIncreased;
        }
    }
}