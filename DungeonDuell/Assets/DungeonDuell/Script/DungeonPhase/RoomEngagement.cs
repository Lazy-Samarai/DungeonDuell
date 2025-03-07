using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace dungeonduell
{
    public class RoomEngagement : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Collider2D> onPlayerRoomEnter;
        [SerializeField] private UnityEvent<Collider2D> onPlayerRoomExit;
        
        [SerializeField] public UnityEvent<Collider2D> onPlayerRoomInnerEnter;
        [SerializeField] public UnityEvent<Collider2D> onPlayerRoomInnerExit;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 10)
            {
                onPlayerRoomEnter.Invoke(collision);
            }


        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 10)
            {
                onPlayerRoomExit.Invoke(collision);
            }

        }
    }
}
