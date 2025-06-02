using System;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using Unity.VisualScripting;

namespace dungeonduell
{
    public class AudioManagement : MonoBehaviour
    {
        public static AudioManagement instance {get; private set;}
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(instance);
            }
            instance = this;
        }

        // If Sound is played on one of TopDown Feedback Stuff use the FMOD Studio Event Emitter
        public void PlayerOneShotSound(EventReference eventReference,Vector3 position)
        {
            RuntimeManager.PlayOneShot(eventReference, position);
        }
        
        
        public EventInstance CreateEventInstance(EventReference eventReference)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            return eventInstance;
        }

        public void CleanUp()
        {
            foreach (StudioEventEmitter studioEventEmitter in FindObjectsByType<StudioEventEmitter>(FindObjectsSortMode.None))
            {
                studioEventEmitter.Stop();
            }
        }

        void OnDestroy()
        {
            CleanUp();
        }

     
    }
}
