using System;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    public class DeactivateOnWin : MonoBehaviour, IObserver
    {
        public void SubscribeToEvents()
        {
            DdCodeEventHandler.weHaveWinner += DeactivateAllAbility;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.weHaveWinner -= DeactivateAllAbility;
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToAllEvents();
        }

        private void DeactivateAllAbility(String p)
        {
            foreach (CharacterAbility characterAbility in GetComponents<CharacterAbility>())
            {
                characterAbility.AbilityPermitted = false;
            }
        }
    }
}