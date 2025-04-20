using System.Collections;
using System.Linq;
using DG.Tweening;
using MoreMountains.TopDownEngine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace dungeonduell
{
    public class TurnManager : MonoBehaviour, IObserver
    {
        private const int SecondsToStart = 3;
        public TextMeshProUGUI playerTurnText;
        public TextMeshProUGUI pressAnyKeyText;
        public CardToHand HandPlayer1;
        public CardToHand HandPlayer2;

        public GameObject player1UI;
        public GameObject player2UI;
        public bool isPlayer1Turn = true;

        public PlayerInput[] _playerInputs;

        private readonly bool[] _playerPlayedAllCards = { false, false };

        private bool awaitingKeyPress;
        private float timeStart;

        private void Start()
        {
            timeStart = Time.time;
            InitializeTurn();
        }

        private void Update()
        {
            if (awaitingKeyPress && Time.time - timeStart > 0.5f &&
                GetActivePlayerInput().actions["Submit"].WasPressedThisFrame())
                BeginPlayerActionPhase();
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
            DDCodeEventHandler.NextPlayerTurn += EndPlayerTurn;
            DDCodeEventHandler.PlayedAllCards += SetPlayerCardsPlayed;
        }

        public void UnsubscribeToAllEvents()
        {
            DDCodeEventHandler.NextPlayerTurn -= EndPlayerTurn;
            DDCodeEventHandler.PlayedAllCards -= SetPlayerCardsPlayed;
        }

        private void InitializeTurn()
        {
            awaitingKeyPress = true;
            playerTurnText.text = "Next Turn: " + (isPlayer1Turn ? "Player 1" : "Player 2");

            if (isPlayer1Turn)
            {
                InputSystem.DisableDevice(_playerInputs[1].user.pairedDevices[0]);
                InputSystem.EnableDevice(_playerInputs[0].user.pairedDevices[0]);
            }
            else
            {
                InputSystem.DisableDevice(_playerInputs[0].user.pairedDevices[0]);
                InputSystem.EnableDevice(_playerInputs[1].user.pairedDevices[0]);
            }

            playerTurnText.gameObject.SetActive(true);
            pressAnyKeyText.gameObject.SetActive(true);
            ToggleHandVisibility(false, false);
        }

        private void BeginPlayerActionPhase()
        {
            if (!awaitingKeyPress) return;

            awaitingKeyPress = false;
            UpdatePlayerTurnText();
            pressAnyKeyText.gameObject.SetActive(false);
            ToggleHandVisibility(isPlayer1Turn, !isPlayer1Turn);

            StartCoroutine(DelayedFirstSelectable());
        }


        private IEnumerator DelayedFirstSelectable()
        {
            yield return null;
            if (isPlayer1Turn) HandPlayer1.FirstSelectable();
            else HandPlayer2.FirstSelectable();
        }

        private void UpdatePlayerTurnText()
        {
            if (playerTurnText != null)
                playerTurnText.text = "Current Turn: " + (isPlayer1Turn ? "Player 1" : "Player 2");
        }

        public void EndPlayerTurn()
        {
            isPlayer1Turn = !isPlayer1Turn;

            if (_playerPlayedAllCards.All(played => played))
                InnitGameCountDown();
            else
                // Verz�gere das Initialisieren des neuen Zuges, um sicherzustellen, dass awaitingKeyPress korrekt gesetzt ist
                Invoke(nameof(InitializeTurn), 0.1f);
        }

        // Neue Methode zum Umschalten der Handkartenanzeige
        private void ToggleHandVisibility(bool showForPlayer1, bool showForPlayer2)
        {
            HandPlayer1.ShowHideDeck(!showForPlayer1);
            HandPlayer2.ShowHideDeck(!showForPlayer2);

            SlidePlayerSprite(player1UI, showForPlayer1);
            SlidePlayerSprite(player2UI, showForPlayer2);
        }


        public void InnitGameCountDown()
        {
            InputSystem.EnableDevice(_playerInputs[0].user.pairedDevices[0]);
            InputSystem.EnableDevice(_playerInputs[1].user.pairedDevices[0]);
            StartCoroutine(StartCountDown());
        }

        private IEnumerator StartCountDown()
        {
            pressAnyKeyText.gameObject.SetActive(true);
            pressAnyKeyText.fontSize *= 2;
            pressAnyKeyText.text = "Make Ready";
            for (var i = SecondsToStart; i > 0; i--)
            {
                yield return new WaitForSeconds(1f);
                pressAnyKeyText.text = "Start in " + i;
            }

            yield return new WaitForSeconds(1f);
            pressAnyKeyText.text = "Loading...";
            FindFirstObjectByType<SceneLoading>().ToTheDungeon();
        }

        private void SetPlayerCardsPlayed(bool player1)
        {
            _playerPlayedAllCards[player1 ? 0 : 1] = true;
        }

        private PlayerInput GetActivePlayerInput()
        {
            var neededID = isPlayer1Turn ? "Player1" : "Player2";
            foreach (var input in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
            {
                var manager = input.GetComponent<InputSystemManagerEventsBased>();
                if (manager != null && manager.PlayerID == neededID)
                    return input;
            }

            return null;
        }

        private void SlidePlayerSprite(GameObject uiElement, bool show, float hiddenY = -550f, float visibleY = 0f)
        {
            if (uiElement == null) return;

            var rect = uiElement.GetComponent<RectTransform>();
            if (show)
            {
                uiElement.SetActive(true);
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, hiddenY);
                rect.DOAnchorPosY(visibleY, 0.5f).SetEase(Ease.OutCubic); // DOTween
            }
            else
            {
                rect.DOAnchorPosY(hiddenY, 0.5f).SetEase(Ease.InCubic).OnComplete(() => uiElement.SetActive(false));
            }
        }
    }
}