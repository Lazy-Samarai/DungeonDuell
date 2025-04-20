using UnityEngine;

namespace dungeonduell
{
    public class RoomEngagementInner : MonoBehaviour
    {
        private RoomEngagement roomEngagement;

        // Start is called before the first frame update
        private void Start()
        {
            roomEngagement = GetComponentInParent<RoomEngagement>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 10) roomEngagement.onPlayerRoomInnerEnter.Invoke(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 10) roomEngagement.onPlayerRoomInnerExit.Invoke(collision);
        }
    }
}