using UnityEngine;

namespace dungeonduell
{
    public class ProximityTooltipTrigger : MonoBehaviour
    {
        [TextArea] public string tooltipText = "Standard-Tooltip";
        public float triggerDistance = 2.5f;

        private Transform[] players;
        private TooltipController tooltipController;
        private bool tooltipVisible;

        private void Start()
        {
            tooltipController = FindObjectOfType<TooltipController>();
            if (tooltipController == null)
            {
                Debug.LogError("[TooltipTrigger] Kein TooltipController gefunden!");
            }

            // Spieler suchen (Player1 + Player2)
            GameObject[] player1 = GameObject.FindGameObjectsWithTag("Player1");
            GameObject[] player2 = GameObject.FindGameObjectsWithTag("Player2");

            players = new Transform[player1.Length + player2.Length];

            for (int i = 0; i < player1.Length; i++)
            {
                players[i] = player1[i].transform;
                Debug.Log("[TooltipTrigger] Player1 gefunden: " + players[i].name);
            }

            for (int i = 0; i < player2.Length; i++)
            {
                players[i + player1.Length] = player2[i].transform;
                Debug.Log("[TooltipTrigger] Player2 gefunden: " + players[i + player1.Length].name);
            }

            if (players.Length == 0)
            {
                Debug.LogWarning("[TooltipTrigger] Keine Spieler gefunden!");
            }
        }

        private void Update()
        {
            if (players == null || tooltipController == null) return;

            bool anyPlayerNear = false;

            foreach (var player in players)
            {
                float dist = Vector3.Distance(player.position, transform.position);
                // 💬 Debug: Zeige jede Distanz zur Maske
                Debug.Log("[TooltipTrigger] Distanz zu " + player.name + ": " + dist);

                if (dist <= triggerDistance)
                {
                    anyPlayerNear = true;
                    break;
                }
            }

            if (anyPlayerNear && !tooltipVisible)
            {
                Debug.Log("[TooltipTrigger] Spieler in Reichweite – Tooltip anzeigen.");
                tooltipController.ShowTooltip(tooltipText, transform.position + Vector3.up * 0.5f);
                tooltipVisible = true;
            }
            else if (!anyPlayerNear && tooltipVisible)
            {
                Debug.Log("[TooltipTrigger] Spieler hat Bereich verlassen – Tooltip ausblenden.");
                tooltipController.HideTooltip();
                tooltipVisible = false;
            }
        }
    }
}
