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
        public CinemachineVirtualCamera transitionCamera;
        public TextMeshProUGUI playerTurnText;
        public TextMeshProUGUI pressAnyKeyText;
        public TileClickHandler tileClickHandler;

        private bool awaitingKeyPress = false;
        public bool isPlayer1Turn = true; // Öffentlich gemacht

        void Start()
        {
            if (tileClickHandler == null)
                tileClickHandler = FindObjectOfType<TileClickHandler>();

            // Initiale Kamerasetup
            UpdateCameras();
        }

        void Update()
        {
            if (awaitingKeyPress && Input.anyKeyDown)
            {
                EndPlayerTurn();
            }
        }

        public void EndPlayerTurn()
        {
            isPlayer1Turn = !isPlayer1Turn;
            playerTurnText.text = "Current Turn: " + (isPlayer1Turn ? "Player 1" : "Player 2");
            UpdateCameras();

            tileClickHandler.ChangePlayer(); // Aktualisiere die Spieleransicht
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
    }
}
