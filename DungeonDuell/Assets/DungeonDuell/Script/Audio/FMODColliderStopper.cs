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
            fmodInstance = RuntimeManager.CreateInstance(fmodShootEvent);

            // Wichtig: verknüpft das Event mit diesem GameObject
            RuntimeManager.AttachInstanceToGameObject(fmodInstance, gameObject, GetComponent<Rigidbody2D>());


            // Optional: Wenn du Rigidbody2D nicht nutzt:
            // RuntimeManager.AttachInstanceToGameObject(fmodInstance, transform, null);
        }

        public void startShootSFX()
        {
            if (fmodInstance.isValid())
                fmodInstance.start();
        }

        public void stopShootSFX()
        {
            if (fmodInstance.isValid())
            {
                fmodInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                fmodInstance.release();
            }
        }

        void OnDestroy()
        {
            if (fmodInstance.isValid())
            {
                fmodInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                fmodInstance.release(); // nicht auskommentieren
            }
        }
    }
}
