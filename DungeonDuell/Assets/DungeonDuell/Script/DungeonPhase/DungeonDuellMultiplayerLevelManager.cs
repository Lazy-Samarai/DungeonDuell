using dungeonduell;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        Health,
        AttackSpeed
    }

    // Partically Copy from Grasslands 
    public class DungeonDuellMultiplayerLevelManager : MultiplayerLevelManager, MMEventListener<CoinEvent>
    {
        public struct DDPoints
        {
            public string PlayerID;
            public int Points;
            public int Level;
            public int CoinsForNextLevel;
        }


        [Header("Bindings")]
        /// An array to store each player's points 
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

        public dungeonduell.SequenceMang sequenceMang;

        const string playerNamebase = "Player";

        PlayerDataManager playerDataManager;

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
                Points[i].CoinsForNextLevel = 1; // Startkosten 
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
                        data.Health = health[i].MaximumHealth;
                        if (weapon[i] != null)
                        {
                            data.AttackSpeed = weapon[i].TimeBetweenUses;
                        }
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
                        health[i].MaximumHealth = data.Health;

                        if (weapon[i] != null)
                        {
                            weapon[i].TimeBetweenUses = data.AttackSpeed;
                        }
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
            base.OnPlayerDeath(playerCharacter);
            int aliveCharacters = 0;
            int i = 0;

            if (playerCharacter.PlayerID == "Player1")
            {
                playerDataManager.PlayerDataList[0].RemainingLive--;
                if (playerDataManager.PlayerDataList[0].RemainingLive <= 0)
                {
                    WinnerID = "Player2";
                    StartCoroutine(GameOver());
                }
            }
            if (playerCharacter.PlayerID == "Player2")
            {
                playerDataManager.PlayerDataList[1].RemainingLive--;
                if (playerDataManager.PlayerDataList[1].RemainingLive <= 0)
                {
                    WinnerID = "Player1";
                    StartCoroutine(GameOver());
                }
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

        /// <summary> 
        /// On update, we update our countdowns and check for input if we're in game over state 
        /// </summary> 
        public virtual void Update()
        {
            CheckForGameOver();
            if (weapon[0] == null && weapon[1] == null)
            {
                weapon[0] = GetPlayerWeapon(1);
                weapon[1] = GetPlayerWeapon(2);
                SynchronizeFromPlayerDataManager();
                TopDownEngineEvent.Trigger(TopDownEngineEventTypes.Repaint, null);
            }

        }

        protected virtual void CheckForGameOver()
        {
            if (_gameOver)
            {
                if ((Input.GetButton("Player1_Jump"))
                     || (Input.GetButton("Player2_Jump"))
                     || (Input.GetButton("Player3_Jump"))
                     || (Input.GetButton("Player4_Jump")))
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
                        LevelUPID = Points[i].PlayerID;
                        TopDownEngineEvent.Trigger(TopDownEngineEventTypes.LevelUp, null);
                    }
                }
            }
        }

        public void ApplyLevelUp(LevelUpOptions option)
        {

            for (int i = 0; i < Points.Length; i++)
            {
                if (Points[i].PlayerID == LevelUPID)
                {

                    Points[i].Points -= Points[i].CoinsForNextLevel;
                    Points[i].CoinsForNextLevel *= 2; // Kosten verdoppeln 
                    Points[i].Level++;
                    TopDownEngineEvent.Trigger(TopDownEngineEventTypes.Repaint, null);

                    switch (option)
                    {
                        case LevelUpOptions.Speed:
                            ApplySpeedIncrease(LevelUPID);
                            break;
                        case LevelUpOptions.Health:
                            ApplyHealthIncrease(LevelUPID);
                            break;
                        case LevelUpOptions.AttackSpeed:
                            ApplyAttackSpeedIncrease(LevelUPID);
                            break;
                    }

                    if (Points[i].Points < Points[i].CoinsForNextLevel)
                    {
                        TopDownEngineEvent.Trigger(TopDownEngineEventTypes.NoLevelUp, null);
                    }
                }
            }
        }

        private void ApplySpeedIncrease(string playerID)
        {
            //CharacterMovement movement = GetPlayerMovement(playerID); 
            if (walking != null)
            {
                if (playerID == "Player1")
                {
                    walking[0].WalkSpeed += 1.0f;
                    walking[0].MovementSpeed += 1.0f;
                    running[0].RunSpeed += 1.0f;
                }
                else if (playerID == "Player2")
                {
                    walking[1].WalkSpeed += 1.0f;
                    walking[1].MovementSpeed += 1.0f;
                    running[1].RunSpeed += 1.0f;
                }
                //case switches f�r die Sachen 
            }
        }

        private void ApplyHealthIncrease(string playerID)
        {
            //Character character = GetPlayerCharacter(playerID); 
            //Health health = character?.GetComponent<Health>(); 
            if (health != null)
            {
                if (playerID == "Player1")
                {
                    health[0].MaximumHealth += 10;
                    health[0].SetHealth(Mathf.Min(health[0].CurrentHealth + 10, health[0].MaximumHealth));
                }
                else if (playerID == "Player2")
                {
                    health[1].MaximumHealth += 10;
                    health[1].SetHealth(Mathf.Min(health[1].CurrentHealth + 10, health[1].MaximumHealth));
                }
            }
        }

        private void ApplyAttackSpeedIncrease(string playerID)
        {
            //ProjectileWeapon weapon = GetPlayerWeapon(playerID); 
            if (weapon[0] != null && weapon[1] != null)
            {
                if (playerID == "Player1")
                {
                    weapon[0].TimeBetweenUses *= 0.9f;
                }
                else if (playerID == "Player2")
                {
                    weapon[1].TimeBetweenUses *= 0.9f; // Schnellere Angriffe 
                }
            }
            else
            {
                weapon[0] = GetPlayerWeapon(1);
                weapon[1] = GetPlayerWeapon(2);
                ApplyAttackSpeedIncrease(playerID);
            }
        }

        private CharacterMovement GetPlayerMovement(int i)
        {
            foreach (CharacterMovement movement in FindObjectsOfType<CharacterMovement>())
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
            foreach (CharacterRun run in FindObjectsOfType<CharacterRun>())
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
            foreach (Character character in FindObjectsOfType<Character>())
            {
                if (character.PlayerID == (playerNamebase + i))
                {
                    return character;
                }
            }
            return null;
        }

        private Health GetPlayerHealth(int i)
        {
            foreach (Health health in FindObjectsOfType<Health>())
            {
                if (health.GetComponent<Character>().PlayerID == (playerNamebase + i))
                {
                    return health;
                }
            }
            return null;
        }

        private ProjectileWeapon GetPlayerWeapon(int i)
        {
            foreach (Character character in FindObjectsOfType<Character>())
            {
                if (character.PlayerID == (playerNamebase + i))
                {
                    return character.GetComponentInChildren<ProjectileWeapon>();
                }
            }
            return null;
        }


        /// <summary> 
        /// Starts listening for pickable item events 
        /// </summary> 
        protected override void OnEnable()
        {
            base.OnEnable();
            this.MMEventStartListening<CoinEvent>();
        }

        /// <summary> 
        /// Stops listening for pickable item events 
        /// </summary> 
        protected override void OnDisable()
        {
            SavePlayerStates();
            base.OnDisable();
            this.MMEventStopListening<CoinEvent>();
        }
    }
}