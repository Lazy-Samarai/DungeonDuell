using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace dungeonduell
{
    public class WinnerScreenController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RectTransform winnerScreenPanel;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private GameObject[] minimaps;
        [SerializeField] private GameObject buttonContainer;
        [SerializeField] private GameObject deadMask;
        [SerializeField] private GameObject splitter;

        [Header("SplitHUD Container")]
        [SerializeField] private RectTransform splitHUDContainer;

        [Header("Settings")]
        [SerializeField] private float delayBeforeExpand = 2f;
        [SerializeField] private float expandDuration = 1f;
        [SerializeField] private Color player1Color = Color.red;
        [SerializeField] private Color player2Color = Color.magenta;

        [Header("Player Settings")]
        [SerializeField] private bool isPlayer1Screen;

        private void OnEnable()
        {
            Debug.Log($"WinnerScreen aktiviert für: {(isPlayer1Screen ? "Spieler 1" : "Spieler 2")}");
            SetupWinnerScreen();
        }

        private void SetupWinnerScreen()
        {
            // Minimaps ausblenden
            foreach (var minimap in minimaps)
            {
                if (minimap != null)
                    minimap.SetActive(false);
            }

            // Hintergrundfarbe setzen
            backgroundImage.color = isPlayer1Screen ? player1Color : player2Color;

            // Buttons am Anfang ausblenden
            if (buttonContainer != null)
                buttonContainer.SetActive(false);

            // WinnerScreen bleibt erstmal auf halbem Bereich (keine Veränderung)

            // Starte die Expansion
            StartCoroutine(ExpandWinnerScreen());
        }

        private IEnumerator ExpandWinnerScreen()
        {
            Debug.Log("Starte Expansion...");

            // Warten vor Expansion
            Debug.Log("Time.timeScale ist aktuell: " + Time.timeScale);

            yield return new WaitForSeconds(delayBeforeExpand);

            // === Hier wird auf Fullscreen umgestellt ===

            if (splitHUDContainer != null)
            {
                // SplitHUDContainer über gesamten Screen ziehen
                splitHUDContainer.offsetMin = Vector2.zero;
                splitHUDContainer.offsetMax = Vector2.zero;
                Debug.Log("SplitHUDContainer auf Vollbild gesetzt!");
            }

            if (winnerScreenPanel != null)
            {
                // WinnerScreen Panel erweitern
                var offsetMin = winnerScreenPanel.offsetMin;
                var offsetMax = winnerScreenPanel.offsetMax;

                if (isPlayer1Screen)
                {
                    // Spieler 1 (linke Seite), erweitere nach links
                    offsetMin.x = 0;
                }
                else
                {
                    // Spieler 2 (rechte Seite), erweitere nach rechts
                    offsetMax.x = 0;
                }
                

                winnerScreenPanel.offsetMin = offsetMin;
                winnerScreenPanel.offsetMax = offsetMax;

                Debug.Log("WinnerScreenPanel erweitert!");
            }
            // Hier neue Sachen deaktivieren
            if (deadMask != null)
            {
                deadMask.SetActive(false);
                Debug.Log("DeadMask deaktiviert!");
            }

            if (splitter != null)
            {
                splitter.SetActive(false);
                Debug.Log("Splitter deaktiviert!");
            }


            // Buttons einblenden
            if (buttonContainer != null)
                buttonContainer.SetActive(true);

            Debug.Log("Expansion abgeschlossen!");
            
        }

        // === Button Methoden ===

        public void OnMainMenuButtonPressed()
        {
            Debug.Log("Zurück ins Hauptmenü!");
            // TODO: Lade Hauptmenü Szene hier
        }

        public void OnRestartButtonPressed()
        {
            Debug.Log("Neustart des Spiels!");
            // TODO: Lade aktuelle Szene neu
        }
    }
}
