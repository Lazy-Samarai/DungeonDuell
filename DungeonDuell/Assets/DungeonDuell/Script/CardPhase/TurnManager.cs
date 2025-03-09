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

        public TestControllMouseOver CourserSour1;
        public TestControllMouseOver CourserSour2;

        private bool awaitingKeyPress = false;

        public bool isPlayer1Turn = true;

        void Start()
        {
            InitializeTurn(); // Startet den ersten Spielzug
        }

        void Update()
        {
            if (awaitingKeyPress && Input.anyKeyDown)
            {
                BeginPlayerActionPhase();
            }
        }

        void InitializeTurn()
        {
            awaitingKeyPress = true; // Setzt den Tastendruck f�r jeden neuen Zug voraus
            // Setze die Anzeige f�r den Zugbeginn
            playerTurnText.text = "Current Turn: " + (isPlayer1Turn ? "Player 1" : "Player 2");
            playerTurnText.gameObject.SetActive(true);
            pressAnyKeyText.gameObject.SetActive(true);

            // Versteckt beide Handkarten zu Beginn des Zuges
            ToggleHandVisibility(false, false);
        }

        void BeginPlayerActionPhase()
        {
            if (!awaitingKeyPress)
            {
                return; // Verhindert, dass die Methode ungewollt ausgef�hrt wird
            }

            awaitingKeyPress = false; // Nach Tastendruck setzen wir auf false


            // Verberge den Schriftzug und die Aufforderung
            playerTurnText.gameObject.SetActive(false);
            pressAnyKeyText.gameObject.SetActive(false);

            // Zeigt die Handkarten f�r den aktuellen Spieler an
            ToggleHandVisibility(isPlayer1Turn, !isPlayer1Turn);
            
        }
        
        public void EndPlayerTurn()
        {
            isPlayer1Turn = !isPlayer1Turn;

            // Verz�gere das Initialisieren des neuen Zuges, um sicherzustellen, dass awaitingKeyPress korrekt gesetzt ist
            ToggleCursor(isPlayer1Turn);
            Invoke(nameof(InitializeTurn), 0.1f);
        } 

        // Neue Methode zum Umschalten der Handkartenanzeige
        private void ToggleHandVisibility(bool showForPlayer1, bool showForPlayer2)
        {
            HandPlayer1.ShowHideDeck(!showForPlayer1);
            HandPlayer2.ShowHideDeck(!showForPlayer2);
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
