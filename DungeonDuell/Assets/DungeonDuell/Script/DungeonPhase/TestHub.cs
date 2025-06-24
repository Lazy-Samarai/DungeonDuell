using System.Text.RegularExpressions;
using dungeonduell;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MoreMountains.TopDownEngine
{
    public class TestHub : TopDownMonoBehaviour, MMEventListener<TopDownEngineEvent>
    {
        [FormerlySerializedAs("PlayerID")]
        [Tooltip("The playerID associated to this HUD")]
        public string playerID = "Player1";

        [FormerlySerializedAs("HealthBar")]
        [Tooltip("the progress bar to use to show the healthbar")]
        public MMProgressBar healthBar;

        [FormerlySerializedAs("PlayerName")]
        [Tooltip("the Text comp to use to display the player name")]
        public TMPro.TextMeshProUGUI playerName;

        [FormerlySerializedAs("AvatarBar")]
        [Tooltip("the radial progress bar to put around the avatar")]
        public MMProgressBar avatarBar;

        [FormerlySerializedAs("CoinCounter")]
        [Tooltip("the counter used to display coin amounts")]
        public TMPro.TextMeshProUGUI coinCounter;

        [FormerlySerializedAs("CoinForNextLevelCounter")]
        [Tooltip("the counter used to display coin amounts needed for level Up")]
        public TMPro.TextMeshProUGUI coinForNextLevelCounter;

        [FormerlySerializedAs("LevelUpNowText")]
        public TMPro.TextMeshProUGUI levelUpNowText;


        [Tooltip("the badge UI element showing level up count in red circle")]
        public LevelUpBadgeUI levelUpBadgeUI;

        public bool canLevelUp;

        [FormerlySerializedAs("LevelUpPanel")]
        [Tooltip("the screen to display if the target levels up")]
        public LevelUpPanel levelUpPanel;

        public bool menuShowing;

        protected virtual void Start()
        {
            coinCounter.text = "0";
            coinForNextLevelCounter.text = "1";
        }

        protected virtual void OnEnable()
        {
            DdCodeEventHandler.LevelUpAvailable += LevelPossible;
            this.MMEventStartListening();
        }

        protected virtual void OnDisable()
        {
            DdCodeEventHandler.LevelUpAvailable -= LevelPossible;
            this.MMEventStopListening();
        }

        public virtual void OnMMEvent(TopDownEngineEvent tdEvent)
        {
            switch (tdEvent.EventType)
            {
                case TopDownEngineEventTypes.PlayerDeath:
                    if (tdEvent.OriginCharacter.PlayerID == playerID)
                    {
                        // Optional: death mask handling
                    }
                    break;

                case TopDownEngineEventTypes.Repaint:
                    foreach (var points in ((LevelManager.Instance as DungeonDuellMultiplayerLevelManager)!).Points)
                        if (points.PlayerID == playerID)
                        {
                            coinCounter.text = points.Points.ToString();
                            coinForNextLevelCounter.text = points.CoinsForNextLevel.ToString();
                        }
                    break;

                case TopDownEngineEventTypes.GameOver:
                    var winnerID = (LevelManager.Instance as DungeonDuellMultiplayerLevelManager)?.WinnerID;
                    bool isWinner = playerID == winnerID;

                    if (isWinner)
                    {
                        var winnerController = GameObject.FindFirstObjectByType<CentralWinnerScreenController>();

                        if (winnerController != null)
                        {
                            bool player1Won = playerID == "Player1";
                            winnerController.ShowWinnerScreen(player1Won);
                        }
                        else
                        {
                            Debug.LogWarning("CentralWinnerScreenController nicht gefunden!");
                        }
                    }
                    break;
            }
        }

        public void LevelPossible(int id, int count)
        {
            var myPlayerIndex = int.Parse(playerID[^1].ToString()) - 1;
            if (myPlayerIndex == id)
            {
                canLevelUp = count > 0;
                levelUpNowText.gameObject.SetActive(canLevelUp);
                levelUpNowText.text = "LEVEL UP";

                if (levelUpBadgeUI != null)
                {
                    levelUpBadgeUI.SetLevelUpCount(count);
                }
            }
        }

        public void ShowMenuTry()
        {
            if (canLevelUp)
            {
                if (!menuShowing)
                    levelUpPanel.ShowLevelUpMenu(true);
                else
                    levelUpPanel.ShowLevelUpMenu(false);
                menuShowing = !menuShowing;
            }
        }
    }
}
