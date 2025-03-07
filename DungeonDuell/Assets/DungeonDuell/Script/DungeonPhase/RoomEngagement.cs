using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace dungeonduell
{
    public class RoomEngagement : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Collider2D> onRoomEnter;
        [SerializeField] private UnityEvent<Collider2D> onRoomExit;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            onRoomEnter.Invoke(collision);

        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            onRoomExit.Invoke(collision);
        }
    }
}
