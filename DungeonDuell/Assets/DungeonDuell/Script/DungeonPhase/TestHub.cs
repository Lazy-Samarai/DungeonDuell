using System.Text.RegularExpressions;
using dungeonduell;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

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
        public LevelUPPanel LevelUpPanel;

        public bool menuShowing;

        protected virtual void Start()
        {
            CoinCounter.text = "0";
            CoinForNextLevelCounter.text = "1";
            DeadMask.gameObject.SetActive(false);
            WinnerScreen.gameObject.SetActive(false);
        }

        /// <summary>
        ///     OnDisable, we start listening to events.
        /// </summary>
        protected virtual void OnEnable()
        {
            DDCodeEventHandler.LevelUpAvailable += LevelPossible;
            this.MMEventStartListening();
        }

        /// <summary>
        ///     OnDisable, we stop listening to events.
        /// </summary>
        protected virtual void OnDisable()
        {
            DDCodeEventHandler.LevelUpAvailable -= LevelPossible;
            this.MMEventStopListening();
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
                        StartCoroutine(MMFade.FadeCanvasGroup(DeadMask, 0.5f, 0.8f));
                    }

                    break;
                case TopDownEngineEventTypes.Repaint:
                    foreach (var points in (LevelManager.Instance as DungeonDuellMultiplayerLevelManager).Points)
                        if (points.PlayerID == PlayerID)
                        {
                            CoinCounter.text = points.Points.ToString();
                            CoinForNextLevelCounter.text = points.CoinsForNextLevel.ToString();
                        }

                    break;
                case TopDownEngineEventTypes.GameOver:
                    if (PlayerID == (LevelManager.Instance as DungeonDuellMultiplayerLevelManager).WinnerID)
                    {
                        WinnerScreen.gameObject.SetActive(true);
                        WinnerScreen.alpha = 0f;
                        StartCoroutine(MMFade.FadeCanvasGroup(WinnerScreen, 0.5f, 0.8f));
                    }

                    break;
            }
        }

        public void LevelPossible(int id, int count)
        {
            var myPlayerIndex = int.Parse(PlayerID[PlayerID.Length - 1].ToString()) - 1;
            if (myPlayerIndex == id)
            {
                canLevelUp = count > 0;
                LevelUpNowText.gameObject.SetActive(canLevelUp);
                LevelUpNowText.text =
                    Regex.Replace(LevelUpNowText.text, "\\s*\\(.*?\\)", "").Trim(); // remove old "(X)"
                LevelUpNowText.text += $"({count})";
            }
        }

        public void ShowMenuTry()
        {
            if (canLevelUp)
            {
                if (!menuShowing)
                    LevelUpPanel.ShowLevelUpMenu(true);
                else
                    LevelUpPanel.ShowLevelUpMenu(false);
                menuShowing = !menuShowing;
            }
        }
    }
}