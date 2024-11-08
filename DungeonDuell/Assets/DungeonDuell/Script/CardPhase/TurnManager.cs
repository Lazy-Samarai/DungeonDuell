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
            StartNewTurn(); // Startet den ersten Spielzug
        }

        void Update()
        {
            if (awaitingKeyPress && Input.anyKeyDown)
            {
                Debug.Log("Tastendruck erkannt, StartPlayerTurn wird aufgerufen");
                StartPlayerTurn();
            }
        }

        void StartNewTurn()
        {
            awaitingKeyPress = true;
            Debug.Log($"Neuer Zug beginnt: {(isPlayer1Turn ? "Spieler 1" : "Spieler 2")}");

            // Setze den Schriftzug entsprechend des aktuellen Spielers
            playerTurnText.text = "Current Turn: " + (isPlayer1Turn ? "Player 1" : "Player 2");
            playerTurnText.gameObject.SetActive(true);
            pressAnyKeyText.gameObject.SetActive(true);
            Debug.Log("Schriftzug und Tastentext aktiviert.");

            // Handkarten beider Spieler deaktivieren
            HandPlayer1.ShowHideDeck(true);
            HandPlayer2.ShowHideDeck(true);
            Debug.Log("Beide Handkarten-Panels deaktiviert.");

            UpdateCameras();
        }


        void StartPlayerTurn()
        {
            awaitingKeyPress = false;
            Debug.Log($"Zug beginnt: {(isPlayer1Turn ? "Spieler 1" : "Spieler 2")}");

            // Verberge den Schriftzug und die Aufforderung
            //playerTurnText.gameObject.SetActive(false);
            //pressAnyKeyText.gameObject.SetActive(false);
            Debug.Log("Schriftzug und Tastentext deaktiviert.");

            // Handkarten des aktuellen Spielers anzeigen
            HandPlayer1.ShowHideDeck(!isPlayer1Turn);
            HandPlayer2.ShowHideDeck(isPlayer1Turn);
            Debug.Log($"Handkarten für {(isPlayer1Turn ? "Spieler 2" : "Spieler 1")} werden ausgeblendet.");
            Debug.Log($"Handkarten für {(isPlayer1Turn ? "Spieler 1" : "Spieler 2")} werden angezeigt.");
        }



        public void EndPlayerTurn()
        {
            isPlayer1Turn = !isPlayer1Turn;
            Debug.Log($"Zug beendet, nächster Spieler: {(isPlayer1Turn ? "Spieler 1" : "Spieler 2")}");

            // Starte den neuen Spielzug
            StartNewTurn();
        }

        private void UpdateCameras()
        {
            Debug.Log($"Kameras aktualisiert: {(isPlayer1Turn ? "Spieler 1" : "Spieler 2")}");

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
    }
}
