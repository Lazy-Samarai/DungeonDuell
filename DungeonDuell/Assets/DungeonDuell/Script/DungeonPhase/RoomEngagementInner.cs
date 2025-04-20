using UnityEngine;

namespace dungeonduell
{
    public class RoomEngagementInner : MonoBehaviour
    {
        private RoomEngagement _roomEngagement;

        // Start is called before the first frame update
        private void Start()
        {
            _roomEngagement = GetComponentInParent<RoomEngagement>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 10) _roomEngagement.onPlayerRoomInnerEnter.Invoke(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 10) _roomEngagement.onPlayerRoomInnerExit.Invoke(collision);
        }
    }
}