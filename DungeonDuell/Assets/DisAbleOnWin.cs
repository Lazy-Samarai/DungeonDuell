using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    public class DisAbleOnWin : MonoBehaviour, IObserver
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
            DdCodeEventHandler.weHaveWinner += DisableWeapon;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.weHaveWinner -= DisableWeapon;
        }

        void DisableWeapon(string s) // it doesn't fire when player in Win Animation
        {
            gameObject.SetActive(false);
        }
    }
}