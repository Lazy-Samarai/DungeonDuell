using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace MoreMountains.TopDownEngine
{
	public class TestHub : TopDownMonoBehaviour, MMEventListener<TopDownEngineEvent>
	{
		/// The playerID associated to this HUD
		[Tooltip("The playerID associated to this HUD")]
		public string PlayerID = "Player1";
		/// the progress bar to use to show the healthbar
		[Tooltip("the progress bar to use to show the healthbar")]
		public MMProgressBar HealthBar;
		/// the Text comp to use to display the player name
		[Tooltip("the Text comp to use to display the player name")]
		public Text PlayerName;
		/// the radial progress bar to put around the avatar
		[Tooltip("the radial progress bar to put around the avatar")]
		public MMProgressBar AvatarBar;
		/// the counter used to display coin amounts
		[Tooltip("the counter used to display coin amounts")]
		public Text CoinCounter;
        [Tooltip("the counter used to display coin amounts needed for level Up")]
        public Text CoinForNextLevelCounter;

		public Text LevelUpNowText;
		public bool canLevelUp;

        /// the mask to use when the target player dies
        [Tooltip("the mask to use when the target player dies")]
		public CanvasGroup DeadMask;
		/// the screen to display if the target player wins
		[Tooltip("the screen to display if the target player wins")]
		public CanvasGroup WinnerScreen;
        [Tooltip("the screen to display if the target levels up")]
        public dungeonduell.LevelUPPanel LevelUpPanel;

		bool menuShowing = false;

		protected virtual void Start()
		{
			CoinCounter.text = "0";
			CoinForNextLevelCounter.text = "1";
			DeadMask.gameObject.SetActive(false);
			WinnerScreen.gameObject.SetActive(false);
		}

		public virtual void OnMMEvent(TopDownEngineEvent tdEvent)
		{
			switch (tdEvent.EventType)
			{
				case TopDownEngineEventTypes.PlayerDeath:
					if (tdEvent.OriginCharacter.PlayerID == PlayerID)
					{
						DeadMask.gameObject.SetActive(true);
						DeadMask.alpha = 0f;
						StartCoroutine(MMFade.FadeCanvasGroup(DeadMask, 0.5f, 0.8f, true));
					}
					break;
				case TopDownEngineEventTypes.Repaint:
					foreach (DungeonDuellMultiplayerLevelManager.DDPoints points in (LevelManager.Instance as DungeonDuellMultiplayerLevelManager).Points)
					{
						if (points.PlayerID == PlayerID)
						{
							CoinCounter.text = points.Points.ToString();
							CoinForNextLevelCounter.text = points.CoinsForNextLevel.ToString() + "  COINS";
						}
					}
					break;
				case TopDownEngineEventTypes.GameOver:
					if (PlayerID == (LevelManager.Instance as DungeonDuellMultiplayerLevelManager).WinnerID)
					{
						WinnerScreen.gameObject.SetActive(true);
						WinnerScreen.alpha = 0f;
						StartCoroutine(MMFade.FadeCanvasGroup(WinnerScreen, 0.5f, 0.8f, true));
					}
					break;

				case TopDownEngineEventTypes.LevelUp:
					if(PlayerID == (LevelManager.Instance as DungeonDuellMultiplayerLevelManager).LevelUPID)
					{
						canLevelUp = true;
                        LevelUpNowText.gameObject.SetActive(canLevelUp);
                    }
					break;

            }

		}
		public void ShowMenuTry()
		{
			if (canLevelUp)
			{
				if (!menuShowing)
					{
						
						LevelUpPanel.ShowLevelUpMenu(true);
						//LevelUpPanel.alpha = 0f;
						//StartCoroutine(MMFade.FadeCanvasGroup(LevelUpPanel, 0.5f, 0.8f, true));
						LevelUpNowText.gameObject.SetActive(false);
					}
					else
					{
					
                        //LevelUpPanel.alpha = 0.8f;
                        //StartCoroutine(MMFade.FadeCanvasGroup(LevelUpPanel, 0.5f, 0f, true));
                        LevelUpNowText.gameObject.SetActive(canLevelUp);
                        LevelUpPanel.ShowLevelUpMenu(false);
                    }
					menuShowing = !menuShowing;
			}
		}
        /// <summary>
        /// OnDisable, we start listening to events.
        /// </summary>
        protected virtual void OnEnable()
		{
			this.MMEventStartListening<TopDownEngineEvent>();
		}

		/// <summary>
		/// OnDisable, we stop listening to events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<TopDownEngineEvent>();
		}
	}
}