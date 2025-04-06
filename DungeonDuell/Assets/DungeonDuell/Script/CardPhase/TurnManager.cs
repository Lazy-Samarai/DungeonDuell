using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MoreMountains.TopDownEngine;
using UnityEngine.InputSystem;


namespace dungeonduell
{
    public class TurnManager : MonoBehaviour, IObserver
    {
        public TextMeshProUGUI playerTurnText;
        public TextMeshProUGUI pressAnyKeyText;
        public CardToHand HandPlayer1;
        public CardToHand HandPlayer2;
        public GameObject canvasEndTurn;

        private bool awaitingKeyPress = false;
        public bool isPlayer1Turn = true;
        private float timeStart;

        private const int SecondsToStart = 3;

        private bool[] _playerPlayedAllCards = {false, false};

        void Start()
        {
            timeStart = Time.time;
            InitializeTurn();
        }

        void Update()
        {
            if (awaitingKeyPress && (Time.time - timeStart > 0.5f) && GetActivePlayerInput().actions["Submit"].WasPressedThisFrame())
            {
                BeginPlayerActionPhase();
            }
        }

        void InitializeTurn()
        {
            awaitingKeyPress = true;
            playerTurnText.text = "Next Turn: " + (isPlayer1Turn ? "Player 1" : "Player 2");
            playerTurnText.gameObject.SetActive(true);
            pressAnyKeyText.gameObject.SetActive(true);
            canvasEndTurn.SetActive(false);
            ToggleHandVisibility(false, false);
        }

        void BeginPlayerActionPhase()
        {
            if (!awaitingKeyPress) return;

            awaitingKeyPress = false;
            UpdatePlayerTurnText();
            pressAnyKeyText.gameObject.SetActive(false);
            ToggleHandVisibility(isPlayer1Turn, !isPlayer1Turn);
            canvasEndTurn.SetActive(true);

            Button skipButton = canvasEndTurn.GetComponentInChildren<Button>();
            if (skipButton != null)
            {
                skipButton.interactable = true;
            }

            StartCoroutine(DelayedFirstSelectable());
        }


        private IEnumerator DelayedFirstSelectable()
        {
            yield return null;
            if (isPlayer1Turn) HandPlayer1.FirstSelectable();
            else HandPlayer2.FirstSelectable();
        }

        void UpdatePlayerTurnText()
        {
            if (playerTurnText != null)
                playerTurnText.text = "Current Turn: " + (isPlayer1Turn ? "Player 1" : "Player 2");
        }
        public void EndPlayerTurn()
        {
            isPlayer1Turn = !isPlayer1Turn;
            if (_playerPlayedAllCards.All(played => played == true))
            {
                InnitGameCountDown();
            }
            else
            {
                // Verz�gere das Initialisieren des neuen Zuges, um sicherzustellen, dass awaitingKeyPress korrekt gesetzt ist
                Invoke(nameof(InitializeTurn), 0.1f);
            }

           
        }
        // Neue Methode zum Umschalten der Handkartenanzeige
        private void ToggleHandVisibility(bool showForPlayer1, bool showForPlayer2)
        {
            HandPlayer1.ShowHideDeck(!showForPlayer1);
            HandPlayer2.ShowHideDeck(!showForPlayer2);
        }

        public void InnitGameCountDown()
        {
            StartCoroutine(StartCountDown());
        }

        IEnumerator StartCountDown()
        {
            pressAnyKeyText.gameObject.SetActive(true);
            pressAnyKeyText.fontSize *= 2;
            pressAnyKeyText.text = "Make Ready";
            for (int i = SecondsToStart ; i > 0; i--)
            {
                yield return new WaitForSeconds(1f);
                pressAnyKeyText.text = "Start in " + i;
            }
            yield return new WaitForSeconds(1f);
            pressAnyKeyText.text = "Loading...";
            FindObjectOfType<SceneLoading>().ToTheDungeon();
        }

        private void SetPlayerCardsPlayed(bool player1)
        {
            _playerPlayedAllCards[player1 ? 0 : 1] = true;
        }

        void OnEnable() => SubscribeToEvents();
        void OnDisable() => UnsubscribeToAllEvents();
        
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

        private PlayerInput GetActivePlayerInput()
        {
            string neededID = isPlayer1Turn ? "Player1" : "Player2";
            foreach (var input in FindObjectsOfType<PlayerInput>())
            {
                var manager = input.GetComponent<InputSystemManagerEventsBased>();
                if (manager != null && manager.PlayerID == neededID)
                    return input;
            }
            return null;
        }
    }
}
