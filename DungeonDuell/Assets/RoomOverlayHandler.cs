using UnityEngine;

namespace dungeonduell
{
    public class RoomOverlayHandler : MonoBehaviour
    {
        [SerializeField] private GameObject coverMapTop; // Das neue graue Overlay

        public void OnPlayerEnter(Collider2D player)
        {
            coverMapTop.SetActive(false); // Overlay ausschalten, wenn Spieler im Raum ist
        }

        public void OnPlayerExit(Collider2D player)
        {
            coverMapTop.SetActive(true); // Overlay wieder einschalten, wenn Spieler rausgeht
        }
    }
}
