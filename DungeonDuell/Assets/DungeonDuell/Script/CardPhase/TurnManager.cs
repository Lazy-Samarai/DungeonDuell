using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.EventSystems;

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

        public TestControllMouseOver CourserSour1;
        public TestControllMouseOver CourserSour2;

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
                return; // Verhindert, dass die Methode ungewollt ausgeführt wird
            }

            awaitingKeyPress = false; // Nach Tastendruck setzen wir auf false
          

            // Verberge den Schriftzug und die Aufforderung
            playerTurnText.gameObject.SetActive(false);
            pressAnyKeyText.gameObject.SetActive(false);

            // Zeigt die Handkarten für den aktuellen Spieler an
            ToggleHandVisibility(isPlayer1Turn, !isPlayer1Turn);
            Invoke(nameof(SelectFirstCard), 0.2f);
        }

        private void SelectFirstCard()
        {
            Transform activeHandPanel = isPlayer1Turn ? HandPlayer1.handPanel : HandPlayer2.handPanel;

            int firstValidIndex = GetFirstValidCardIndex(activeHandPanel);
            if (firstValidIndex != -1)
            {
                GameObject firstCard = activeHandPanel.GetChild(firstValidIndex).gameObject;
                EventSystem.current.SetSelectedGameObject(firstCard);

                DisplayCard firstCardScript = firstCard.GetComponent<DisplayCard>();
                if (firstCardScript != null)
                {
                    firstCardScript.SetHighlight(true);
                    Debug.Log($"✨ Erste Karte {firstCardScript.card.cardName} hervorgehoben!");
                }
            }
            else
            {
                Debug.LogError("❌ Auch nach Wartezeit KEINE Karten gefunden!");
            }
        }

        private int GetFirstValidCardIndex(Transform panel)
        {
            for (int i = 0; i < panel.childCount; i++)
            {
                if (panel.GetChild(i).GetComponent<DisplayCard>() != null)
                {
                    return i;
                }
            }
            return -1;
        }
        public void EndPlayerTurn()
        {
            Debug.Log("EndPlayerTurn aufgerufen");

            isPlayer1Turn = !isPlayer1Turn;
            Debug.Log($"Zug beendet für {(isPlayer1Turn ? "Spieler 1" : "Spieler 2")} - nächster Spieler ist dran");

            // Verzögere das Initialisieren des neuen Zuges, um sicherzustellen, dass awaitingKeyPress korrekt gesetzt ist
            ToggleCursor(isPlayer1Turn);
            Invoke(nameof(InitializeTurn), 0.1f);
           
        }

        private void UpdateCameras()
        {
            //Debug.Log($"Kameras für {(isPlayer1Turn ? "Spieler 1" : "Spieler 2")} aktualisiert");

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
    }
}
