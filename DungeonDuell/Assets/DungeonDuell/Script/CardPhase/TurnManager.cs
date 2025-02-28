using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

namespace dungeonduell
{
    public class TurnManager : MonoBehaviour,IObserver
    {
        public CinemachineVirtualCamera player1Camera;
        public CinemachineVirtualCamera player2Camera;
        public TextMeshProUGUI playerTurnText;
        public TextMeshProUGUI pressAnyKeyText;
        public CardToHand HandPlayer1;
        public CardToHand HandPlayer2;

        public TestControllMouseOver CourserSour1;
        public TestControllMouseOver CourserSour2;

        private bool awaitingKeyPress = false;

        public bool isPlayer1Turn = true;

        void Start()
        {          
            UpdateCameras();
            InitializeTurn(); // Startet den ersten Spielzug
        }

        void Update()
        {
            if (awaitingKeyPress && Input.anyKeyDown)
            {
                Debug.Log("Tastendruck erkannt, BeginPlayerActionPhase wird aufgerufen");
                BeginPlayerActionPhase();
            }
        }

        void InitializeTurn()
        {
            Debug.Log("InitializeTurn aufgerufen");

            awaitingKeyPress = true; // Setzt den Tastendruck f�r jeden neuen Zug voraus
            Debug.Log($"Neuer Zug beginnt f�r {(isPlayer1Turn ? "Spieler 1" : "Spieler 2")} - awaitingKeyPress: {awaitingKeyPress}");

            // Setze die Anzeige f�r den Zugbeginn
            playerTurnText.text = "Current Turn: " + (isPlayer1Turn ? "Player 1" : "Player 2");
            playerTurnText.gameObject.SetActive(true);
            pressAnyKeyText.gameObject.SetActive(true);
            Debug.Log("Spielzug-Text und Tastendruck-Text aktiviert.");

            // Versteckt beide Handkarten zu Beginn des Zuges
            ToggleHandVisibility(false, false);

            UpdateCameras();
        }

        void BeginPlayerActionPhase()
        {
            if (!awaitingKeyPress)
            {
                Debug.LogWarning("BeginPlayerActionPhase wurde aufgerufen, obwohl awaitingKeyPress false ist.");
                return; // Verhindert, dass die Methode ungewollt ausgef�hrt wird
            }

            awaitingKeyPress = false; // Nach Tastendruck setzen wir auf false
            Debug.Log($"BeginPlayerActionPhase aufgerufen f�r {(isPlayer1Turn ? "Spieler 1" : "Spieler 2")} - awaitingKeyPress: {awaitingKeyPress}");

            // Verberge den Schriftzug und die Aufforderung
            playerTurnText.gameObject.SetActive(false);
            pressAnyKeyText.gameObject.SetActive(false);
            Debug.Log("Schriftzug und Tastentext deaktiviert.");

            // Zeigt die Handkarten f�r den aktuellen Spieler an
            ToggleHandVisibility(isPlayer1Turn, !isPlayer1Turn);
        }

        public void EndPlayerTurn()
        {
            Debug.Log("EndPlayerTurn aufgerufen");

            isPlayer1Turn = !isPlayer1Turn;
            Debug.Log($"Zug beendet f�r {(isPlayer1Turn ? "Spieler 1" : "Spieler 2")} - n�chster Spieler ist dran");

            // Verz�gere das Initialisieren des neuen Zuges, um sicherzustellen, dass awaitingKeyPress korrekt gesetzt ist
            ToggleCursor(isPlayer1Turn);
            Invoke(nameof(InitializeTurn), 0.1f);
           
        }

        private void UpdateCameras()
        {
            Debug.Log($"Kameras f�r {(isPlayer1Turn ? "Spieler 1" : "Spieler 2")} aktualisiert");

            if (isPlayer1Turn)
            {
                player1Camera.Priority = 20;
                player2Camera.Priority = 10;
            }
            else
            {
                player1Camera.Priority = 10;
                player2Camera.Priority = 20;
            }
        }

        // Neue Methode zum Umschalten der Handkartenanzeige
        private void ToggleHandVisibility(bool showForPlayer1, bool showForPlayer2)
        {
            HandPlayer1.ShowHideDeck(!showForPlayer1);
            HandPlayer2.ShowHideDeck(!showForPlayer2);

            Debug.Log($"Handkarten umgeschaltet: Spieler 1 sichtbar: {showForPlayer1}, Spieler 2 sichtbar: {showForPlayer2}");
        }
        private void ToggleCursor(bool player1)
        {
            CourserSour1.Set(player1);
            CourserSour2.Set(!player1); 
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
