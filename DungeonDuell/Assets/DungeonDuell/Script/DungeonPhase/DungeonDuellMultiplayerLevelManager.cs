using dungeonduell;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
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

		public dungeonduell.SequenceMang sequenceMang;

        /// <summary>
        /// On init, we initialize our points and countdowns
        /// </summary>

        protected override void Initialization()
		{
		
			base.Initialization();
			WinnerID = "";
            LevelUPID = "";
			Points = new DDPoints[Players.Count];
			int i = 0;
			foreach (Character player in Players)
			{
				Points[i].PlayerID = player.PlayerID;
				Points[i].Points = 0;
                Points[i].Level = 1;
                Points[i].CoinsForNextLevel = 10; // Startkosten
                i++;
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

			dungeonduell.LivesManager livesManager = FindObjectOfType<dungeonduell.LivesManager>();


			if (playerCharacter.PlayerID == "Player1")
            {
				livesManager.livesPlayer1--;
				if(livesManager.livesPlayer1 <= 0)
                {
					WinnerID = "Player2";
					StartCoroutine(GameOver());
				}
			}
			if (playerCharacter.PlayerID == "Player2")
			{
				livesManager.livesPlayer2--;
				if (livesManager.livesPlayer2 <= 0)
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
			/*

			if (aliveCharacters <= 1)
			{
				StartCoroutine(GameOver());
			}
			*/
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
                        TriggerLevelUp(i);
                    }
				}
			}
		}

        protected void TriggerLevelUp(int playerIndex)
        {
            Points[playerIndex].Points -= Points[playerIndex].CoinsForNextLevel;
            Points[playerIndex].CoinsForNextLevel *= 2; // Kosten verdoppeln

            LevelUPID = Points[playerIndex].PlayerID;

            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.LevelUp, null);
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.Repaint, null);
        }


        public void ApplyLevelUp(LevelUpOptions option)
        {

            for (int i = 0; i < Points.Length; i++)
            {
                if (Points[i].PlayerID == _playerID)
                {
                    Points[i].Level++;
                    Debug.Log($"Spieler {_playerID} ist jetzt Level {Points[i].Level}");

                    switch (option)
                    {
                        case LevelUpOptions.Speed:
                            ApplySpeedIncrease(_playerID);
                            break;
                        case LevelUpOptions.Health:
                            ApplyHealthIncrease(_playerID);
                            break;
                        case LevelUpOptions.AttackSpeed:
                            ApplyAttackSpeedIncrease(_playerID);
                            break;
                    }
                }
            }
        }

        private void ApplySpeedIncrease(string playerID)
        {
            CharacterMovement movement = GetPlayerMovement(playerID);
            if (movement != null)
            {
                movement.WalkSpeed += 1.0f;
                movement.MovementSpeed += 1.0f;
            }
        }

        private void ApplyHealthIncrease(string playerID)
        {
            Character character = GetPlayerCharacter(playerID);
            Health health = character?.GetComponent<Health>();
            if (health != null)
            {
                health.MaximumHealth += 10;
                health.SetHealth(Mathf.Min(health.CurrentHealth + 10, health.MaximumHealth));
            }
        }

        private void ApplyAttackSpeedIncrease(string playerID)
        {
            ProjectileWeapon weapon = GetPlayerWeapon(playerID);
            if (weapon != null)
            {
                weapon.TimeBetweenUses *= 0.9f; // Schnellere Angriffe
            }
        }

        private CharacterMovement GetPlayerMovement(string playerID)
        {
            foreach (CharacterMovement movement in FindObjectsOfType<CharacterMovement>())
            {
                if (movement.GetComponent<Character>().PlayerID == playerID)
                {
                    return movement;
                }
            }
            return null;
        }

        private Character GetPlayerCharacter(string playerID)
        {
            foreach (Character character in FindObjectsOfType<Character>())
            {
                if (character.PlayerID == playerID)
                {
                    return character;
                }
            }
            return null;
        }

        private ProjectileWeapon GetPlayerWeapon(string playerID)
        {
            foreach (ProjectileWeapon weapon in FindObjectsOfType<ProjectileWeapon>())
            {
                if (weapon.GetComponent<Character>().PlayerID == playerID)
                {
                    return weapon;
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
			base.OnDisable();
			this.MMEventStopListening<CoinEvent>();
		}
	}
}
