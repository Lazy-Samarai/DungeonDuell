using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace dungeonduell
{
    public class levelUpAvailaible : MonoBehaviour, IObserver
    {
        [SerializeField] private EventReference openLevelUPEvent;

        public void SubscribeToEvents()
        {
            DdCodeEventHandler.NewLevelUpPossible += PlayLevelUpSound;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.NewLevelUpPossible -= PlayLevelUpSound;
        }

        void PlayLevelUpSound(int playerID)
        {
            RuntimeManager.PlayOneShot(openLevelUPEvent, transform.position);
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
