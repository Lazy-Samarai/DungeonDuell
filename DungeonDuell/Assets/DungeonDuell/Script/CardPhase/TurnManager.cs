using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.EventSystems;

namespace dungeonduell
{
    public class TurnManager : MonoBehaviour,IObserver
    {
        public TextMeshProUGUI playerTurnText;
        public TextMeshProUGUI pressAnyKeyText;
        public CardToHand HandPlayer1;
        public CardToHand HandPlayer2;

        private bool awaitingKeyPress = false;

        public bool isPlayer1Turn = true;
        private float timeStart;

        void Start()
        {
            timeStart = Time.time;
            InitializeTurn(); // Startet den ersten Spielzug
        }

        void Update()
        {
            if (awaitingKeyPress && (Time.time - timeStart > 0.5f) && Input.anyKeyDown)
            {
                BeginPlayerActionPhase();
            }
        }

        void InitializeTurn()
        {
            awaitingKeyPress = true; // Setzt den Tastendruck f�r jeden neuen Zug voraus
            // Setze die Anzeige f�r den Zugbeginn
            playerTurnText.text = "Next Turn: " + (isPlayer1Turn ? "Player 1" : "Player 2");
            playerTurnText.gameObject.SetActive(true);
            pressAnyKeyText.gameObject.SetActive(true);

            // Versteckt beide Handkarten zu Beginn des Zuges
            ToggleHandVisibility(false, false);
        }

        void BeginPlayerActionPhase()
        {
            if (!awaitingKeyPress)
            {
                return;
            }

            awaitingKeyPress = false;
            UpdatePlayerTurnText();

            pressAnyKeyText.gameObject.SetActive(false);

            // Zeigt die Handkarten für den aktuellen Spieler an
            ToggleHandVisibility(isPlayer1Turn, !isPlayer1Turn);

            // Prüfe, ob der erste Input von einem Controller kam
            if (IsUsingController())
            {
                if (isPlayer1Turn)
                    HandPlayer1.SelectFirstCard();
                else
                    HandPlayer2.SelectFirstCard();
            }
        }

        void UpdatePlayerTurnText()
        {
            if (playerTurnText != null)
            {
                playerTurnText.text = "Current Turn: " + (isPlayer1Turn ? "Player 1" : "Player 2");
            }
        }



        bool IsUsingController()
        {
            string[] controllers = Input.GetJoystickNames();
            if (controllers.Length > 0) // Prüfen, ob Controller angeschlossen ist
            {
                bool stickMoved = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
                bool buttonPressed = Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.JoystickButton1);
                bool mouseMoved = Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0;

                return (stickMoved || buttonPressed) && !mouseMoved; // Verhindert Controller-Erkennung, wenn die Maus bewegt wurde
            }
            return false;
        }



        public void EndPlayerTurn()
        {
            isPlayer1Turn = !isPlayer1Turn;

            // Verz�gere das Initialisieren des neuen Zuges, um sicherzustellen, dass awaitingKeyPress korrekt gesetzt ist
            Invoke(nameof(InitializeTurn), 0.1f);
        } 

        // Neue Methode zum Umschalten der Handkartenanzeige
        private void ToggleHandVisibility(bool showForPlayer1, bool showForPlayer2)
        {
            HandPlayer1.ShowHideDeck(!showForPlayer1);
            HandPlayer2.ShowHideDeck(!showForPlayer2);
        }
        
        void OnEnable()
        {
            SubscribeToEvents();
        }
        void OnDisable()
        {
           UnsubscribeToAllEvents();
        }
        public void SubscribeToEvents()
        {
            DDCodeEventHandler.NextPlayerTurn += EndPlayerTurn;
        }

        public void UnsubscribeToAllEvents()
        {
            DDCodeEventHandler.NextPlayerTurn -= EndPlayerTurn;
        }
    }
}
