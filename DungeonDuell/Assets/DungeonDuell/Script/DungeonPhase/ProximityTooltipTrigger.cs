using Cinemachine;
using UnityEngine;


namespace dungeonduell
{
    public class ProximityTooltipTrigger : MonoBehaviour
    {
        [TextArea] public string tooltipText = "Standard-Tooltip";

        [Header("Player Tags")]
        [SerializeField] [TagField] private string player1Tag = "Player1";
        [SerializeField] [TagField] private string player2Tag = "Player2";

        private TooltipController tooltipControllerPlayer1;
        private TooltipController tooltipControllerPlayer2;

        private Camera cameraPlayer1;
        private Camera cameraPlayer2;

        private void Start()
        {
            // TooltipController für Spieler 1 & 2 suchen
            tooltipControllerPlayer1 = FindTooltipController("TooltipCanvasP1");
            tooltipControllerPlayer2 = FindTooltipController("TooltipCanvasP2");

            // Kameras automatisch finden
            cameraPlayer1 = GameObject.Find("CameraPlayer1")?.GetComponent<Camera>();
            cameraPlayer2 = GameObject.Find("CameraPlayer2")?.GetComponent<Camera>();

            if (tooltipControllerPlayer1 == null || tooltipControllerPlayer2 == null)
                Debug.LogWarning("[TooltipTrigger] TooltipController nicht gefunden!");

            if (cameraPlayer1 == null || cameraPlayer2 == null)
                Debug.LogWarning("[TooltipTrigger] Kamera nicht gefunden!");
        }

        private TooltipController FindTooltipController(string canvasName)
        {
            GameObject canvasGO = GameObject.Find(canvasName);
            if (canvasGO == null) return null;
            return canvasGO.GetComponent<TooltipController>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(player1Tag) && tooltipControllerPlayer1 && cameraPlayer1)
            {
                tooltipControllerPlayer1.ShowTooltip(tooltipText, transform.position + Vector3.up * 1f, cameraPlayer1);
            }
            else if (other.CompareTag(player2Tag) && tooltipControllerPlayer2 && cameraPlayer2)
            {
                tooltipControllerPlayer2.ShowTooltip(tooltipText, transform.position + Vector3.up * 1f, cameraPlayer2);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(player1Tag) && tooltipControllerPlayer1)
            {
                tooltipControllerPlayer1.HideTooltip();
            }
            else if (other.CompareTag(player2Tag) && tooltipControllerPlayer2)
            {
                tooltipControllerPlayer2.HideTooltip();
            }
        }
    }
}
