using UnityEngine;
using TMPro;
using Cinemachine;

namespace dungeonduell
{
    public class ProximityTooltipTrigger : MonoBehaviour
    {
        [Header("Textobjekt im World Space")]
        [SerializeField] private GameObject toolTip;

        [Header("Player Tags")]
        [SerializeField] [TagField] private string player1Tag = "Player1";
        [SerializeField] [TagField] private string player2Tag = "Player2";

        private void Start()
        {
            if (toolTip != null)
                toolTip.SetActive(false);
            else
                Debug.LogWarning("toolTip nicht gesetzt.");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((other.CompareTag(player1Tag) || other.CompareTag(player2Tag)) && toolTip != null)
            {
                toolTip.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if ((other.CompareTag(player1Tag) || other.CompareTag(player2Tag)) && toolTip != null)
            {
                toolTip.SetActive(false);
            }
        }
    }
}
