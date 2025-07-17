using FMODUnity;
using UnityEngine;
using FMOD.Studio;


namespace dungeonduell
{
    public class FMODColliderStopper : MonoBehaviour
    {
        [Header("FMOD Event Reference")]
        public EventReference fmodShootEvent;

        private EventInstance fmodInstance;

        void Start()
        {
            // Event erstellen und starten
            fmodInstance = RuntimeManager.CreateInstance(fmodShootEvent);
            fmodInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        }

        public void startShootSFX()
        {
            fmodInstance.start();
        }
        public void stopShootSFX()
        {
            fmodInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            fmodInstance.release();
        }
        void OnDestroy()
        {
            if (fmodInstance.isValid())
            {
                fmodInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                //fmodInstance.release();
            }
        }
    }
}
