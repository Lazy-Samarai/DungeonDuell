using UnityEngine;

namespace dungeonduell
{
    public class DestroyOnGameReset : MonoBehaviour, IObserver
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
            DdCodeEventHandler.GameReset += OnGameReset;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.GameReset -= OnGameReset;
        }

        private void OnGameReset()
        {
            Destroy(gameObject);
        }
    }
}