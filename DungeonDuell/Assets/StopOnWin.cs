using System;
using MoreMountains.TopDownEngine;
using Unity.VisualScripting;
using UnityEngine;

namespace dungeonduell
{
    public class StopOnWin : MonoBehaviour, IObserver
    {
        void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToAllEvents();
        }

        public void SubscribeToEvents()
        {
            DdCodeEventHandler.weHaveWinner += StopMove;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.weHaveWinner -= StopMove;
        }

        private void StopMove(string _)
        {
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            GetComponent<TopDownController2D>().enabled = false;
        }
    }
}