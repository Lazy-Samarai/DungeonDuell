using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

namespace dungeonduell
{
    public class TurnManager : MonoBehaviour
    {
        public CinemachineVirtualCamera player1Camera;
        public CinemachineVirtualCamera player2Camera;
        public TextMeshProUGUI playerTurnText;
        public TextMeshProUGUI pressAnyKeyText;
        public TileClickHandler tileClickHandler;
        public CardToHand HandPlayer1;
        public CardToHand HandPlayer2;

        private bool awaitingKeyPress = false;
        public bool isPlayer1Turn = true;

        void Start()
        {
            if (tileClickHandler == null)
                tileClickHandler = FindObjectOfType<TileClickHandler>();

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

            awaitingKeyPress = true; // Setzt den Tastendruck für jeden neuen Zug voraus
            Debug.Log($"Neuer Zug beginnt für {(isPlayer1Turn ? "Spieler 1" : "Spieler 2")} - awaitingKeyPress: {awaitingKeyPress}");

            // Setze die Anzeige für den Zugbeginn
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
                return; // Verhindert, dass die Methode ungewollt ausgeführt wird
            }

            awaitingKeyPress = false; // Nach Tastendruck setzen wir auf false
            Debug.Log($"BeginPlayerActionPhase aufgerufen für {(isPlayer1Turn ? "Spieler 1" : "Spieler 2")} - awaitingKeyPress: {awaitingKeyPress}");

            // Verberge den Schriftzug und die Aufforderung
            playerTurnText.gameObject.SetActive(false);
            pressAnyKeyText.gameObject.SetActive(false);
            Debug.Log("Schriftzug und Tastentext deaktiviert.");

            // Zeigt die Handkarten für den aktuellen Spieler an
            ToggleHandVisibility(isPlayer1Turn, !isPlayer1Turn);
        }

        public void EndPlayerTurn()
        {
            Debug.Log("EndPlayerTurn aufgerufen");

            isPlayer1Turn = !isPlayer1Turn;
            Debug.Log($"Zug beendet für {(isPlayer1Turn ? "Spieler 1" : "Spieler 2")} - nächster Spieler ist dran");

            // Verzögere das Initialisieren des neuen Zuges, um sicherzustellen, dass awaitingKeyPress korrekt gesetzt ist
            Invoke(nameof(InitializeTurn), 0.1f);
        }

        private void UpdateCameras()
        {
            Debug.Log($"Kameras für {(isPlayer1Turn ? "Spieler 1" : "Spieler 2")} aktualisiert");

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
    }
}
