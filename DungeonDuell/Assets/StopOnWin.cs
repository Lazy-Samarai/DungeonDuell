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
            DdCodeEventHandler.weHaveWinner += StoppingOnWin;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.weHaveWinner -= StoppingOnWin;
        }

        protected virtual void StoppingOnWin(string _)
        {
            CharacterMovement cm = GetComponent<CharacterMovement>();
            CharacterRun cr = GetComponent<CharacterRun>();
            TopDownController2D controller = GetComponent<TopDownController2D>();
            controller.enabled = false;
            cr.AbilityPermitted = false;
            cm.AbilityPermitted = false;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }
}