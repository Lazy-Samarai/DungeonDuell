using System.Collections;
using System.Collections.Generic;
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
            Invoke(nameof(InitializeTurn), 0.1f);
        }

        private void ToggleHandVisibility(bool showForPlayer1, bool showForPlayer2)
        {
            HandPlayer1.ShowHideDeck(!showForPlayer1);
            HandPlayer2.ShowHideDeck(!showForPlayer2);
        }

        void OnEnable() => SubscribeToEvents();
        void OnDisable() => UnsubscribeToAllEvents();

        public void SubscribeToEvents()
        {
            DDCodeEventHandler.NextPlayerTurn += EndPlayerTurn;
        }

        public void UnsubscribeToAllEvents()
        {
            DDCodeEventHandler.NextPlayerTurn -= EndPlayerTurn;
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
