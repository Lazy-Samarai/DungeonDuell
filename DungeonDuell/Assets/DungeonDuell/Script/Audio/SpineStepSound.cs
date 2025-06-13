using UnityEngine;
using Spine.Unity;
using UnityEngine.Events;
using FMOD.Studio;
using FMODUnity;

public class SpineStepSound : MonoBehaviour
{
    public string spineEventName = ""; // z. B. "Footstep"
    public EventReference fmodFootstepEvent;

    private SkeletonAnimation skeletonAnimation;
    private EventInstance footstepInstance;
    private bool isStepPlaying = false;

    void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationState.Event += HandleSpineEvent;
        }

    }

    private void HandleSpineEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        print(e.Data.Name);
        if (e.Data.Name == spineEventName)
        {
            // Wenn schon ein Sound läuft, stoppen und neu erzeugen
            if (isStepPlaying && footstepInstance.isValid())
            {
                footstepInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                footstepInstance.release();
            }

            footstepInstance = RuntimeManager.CreateInstance(fmodFootstepEvent);
            footstepInstance.start();
            footstepInstance.release();
            isStepPlaying = true;
        }
    }

    public void StopStepSound()
    {
        if (isStepPlaying && footstepInstance.isValid())
        {
            footstepInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            footstepInstance.release();
            isStepPlaying = false;
        }
    }

    private void OnDestroy()
    {
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationState.Event -= HandleSpineEvent;
        }

        if (footstepInstance.isValid())
        {
            footstepInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            footstepInstance.release();
        }
    }
}
