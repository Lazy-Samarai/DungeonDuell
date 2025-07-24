using UnityEngine;
using UnityEngine.Events;

namespace dungeonduell
{
    public class RoomEngagement : MonoBehaviour
    {
        public int RoomIndex { get; set; }
        [SerializeField] private UnityEvent<Collider2D> onPlayerRoomEnter;
        [SerializeField] private UnityEvent<Collider2D> onPlayerRoomExit;

        [SerializeField] public UnityEvent<Collider2D> onPlayerRoomInnerEnter;
        [SerializeField] public UnityEvent<Collider2D> onPlayerRoomInnerExit;

        private GameObject _currentPlayer;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 10)
            {
                if (_currentPlayer == null)
                {
                    _currentPlayer = collision.gameObject;
                }
                else if (collision.gameObject != _currentPlayer)
                {
                    DdCodeEventHandler.Trigger_PlayersInSameRoom();
                }

                onPlayerRoomEnter.Invoke(collision);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 10)
            {
                _currentPlayer = null;
                onPlayerRoomExit.Invoke(collision);
            }
        }

        // Called by Event by Assigning in Editor because an Enemy Room Should only Count as Visited if enemy Are Dead
        public void DeclareRoomEntered()
        {
            DdCodeEventHandler.Trigger_RoomEntered(RoomIndex);
        }
    }
}