using UnityEngine;

namespace dungeonduell
{
    public class FinalDuellAtmoChange : MonoBehaviour, IObserver
    {
        public void SubscribeToEvents()
        {
            DdCodeEventHandler.PlayersInSameRoom += PlayerAreInSameRoom;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.PlayersInSameRoom -= PlayerAreInSameRoom;
        }

        void PlayerAreInSameRoom()
        {
            DdCodeEventHandler.Trigger_AtmosphereLevelChanged(AtmoLevel.Atmo_final);
        }


        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToAllEvents();
        }
    }
}
