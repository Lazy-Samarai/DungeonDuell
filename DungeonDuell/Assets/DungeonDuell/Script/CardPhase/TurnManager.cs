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

          //  UpdateCameras();
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
            Invoke(nameof(SelectFirstCard), 0.2f);
        }

        private void SelectFirstCard()
        {
            Transform activeHandPanel = isPlayer1Turn ? HandPlayer1.handPanel : HandPlayer2.handPanel;

            if (activeHandPanel.childCount > 0)
            {
                GameObject firstCard = activeHandPanel.GetChild(0).gameObject;
                EventSystem.current.SetSelectedGameObject(firstCard);

                DisplayCard firstCardScript = firstCard.GetComponent<DisplayCard>();
                if (firstCardScript != null)
                {
                    firstCardScript.SetHighlight(true);
                    Debug.Log($"Erste Karte {firstCardScript.card.cardName} hervorgehoben!");
                }
            }
            else
            {
                Debug.LogError(" Auch nach Wartezeit KEINE Karten gefunden!");
            }
        }
            public void EndPlayerTurn()
        {
            isPlayer1Turn = !isPlayer1Turn;

            // Verz�gere das Initialisieren des neuen Zuges, um sicherzustellen, dass awaitingKeyPress korrekt gesetzt ist
            ToggleCursor(isPlayer1Turn);
            Invoke(nameof(InitializeTurn), 0.1f);
        }

        private void UpdateCameras()
        {
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
