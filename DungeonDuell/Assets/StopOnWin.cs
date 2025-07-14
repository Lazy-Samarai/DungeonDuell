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
            CharacterMovement cm = GetComponent<CharacterMovement>();
            CharacterRun cr = GetComponent<CharacterRun>();
            cr.AbilityPermitted = false;
            cm.AbilityPermitted = false;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }
}