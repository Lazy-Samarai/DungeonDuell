using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

// TurnManager.cs
namespace dungeonduell
{
    public class TurnManager : MonoBehaviour
    {
        public CinemachineVirtualCamera player1Camera;
        public CinemachineVirtualCamera player2Camera;
        public CinemachineVirtualCamera transitionCamera;
        public TextMeshProUGUI playerTurnText;
        public TextMeshProUGUI pressAnyKeyText;
        public TileClickHandler tileClickHandler; // Hinzugefügt für Spielerwechsel-Logik

        private bool awaitingKeyPress = false;
        private bool isPlayer1Turn = true;

        void Start()
        {
            if (tileClickHandler == null)
                tileClickHandler = FindObjectOfType<TileClickHandler>();

            // Initiale Kamerasetup
            UpdateCameras();
        }

        public void EndPlayerTurn()
        {
            // Diese Methode wird aufgerufen, wenn der Spieler seine Runde abgeschlossen hat
            ShowPlayerTurn(isPlayer1Turn ? "Player 1" : "Player 2");
            StartCoroutine(SwitchToTransitionCamera());
        }

        private IEnumerator SwitchToTransitionCamera()
        {
            // Priorität auf die TransitionCamera setzen
            transitionCamera.Priority = 20;
            player1Camera.Priority = 10;
            player2Camera.Priority = 10;

            // Warten auf die Taste
            awaitingKeyPress = true;
            pressAnyKeyText.gameObject.SetActive(true);

            while (awaitingKeyPress)
            {
                yield return null; // Warte auf die nächste Frame
            }

            SwitchToNextPlayer();
        }

        public void SwitchToNextPlayer()
        {
            // Zurücksetzen der Prioritäten
            transitionCamera.Priority = 10;

            // Spielerwechsel
            isPlayer1Turn = !isPlayer1Turn;
            UpdateCameras();

            // Spielerwechsel im TileClickHandler
            tileClickHandler.Player_1Turn = isPlayer1Turn;
            tileClickHandler.ChangePlayer(isPlayer1Turn);
        }

        private void ShowPlayerTurn(string playerName)
        {
            playerTurnText.text = $"{playerName} ist dran";
            playerTurnText.gameObject.SetActive(true);
        }

        void Update()
        {
            if (awaitingKeyPress && Input.anyKeyDown)
            {
                awaitingKeyPress = false;
                playerTurnText.gameObject.SetActive(false);
                pressAnyKeyText.gameObject.SetActive(false);
            }
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
                player2Camera.Priority = 20;
                player1Camera.Priority = 10;
            }
        }
    }
}
