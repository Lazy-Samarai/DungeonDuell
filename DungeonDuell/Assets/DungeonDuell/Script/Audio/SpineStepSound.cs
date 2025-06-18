using UnityEngine;
using Spine.Unity;
using FMODUnity;
using FMOD.Studio;
using MoreMountains.TopDownEngine;

public class SpineStepSound : MonoBehaviour
{
    [SpineEvent] public string spineEventName = "Step";

    [Header("FMOD Events")]
    public EventReference normalWalk;
    public EventReference normalRun;
    public EventReference lootWalk;
    public EventReference lootRun;

    [Header("Bodenerkennung")]
    public LayerMask groundLayerMask;
    public float raycastDistance = 1f;

    private SkeletonAnimation skeletonAnimation;
    private Character _character;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        _character = GetComponent<Character>();

        if (skeletonAnimation != null)
            skeletonAnimation.AnimationState.Event += HandleSpineEvent;
    }

    private void HandleSpineEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name != spineEventName) return;

        // Prüfe: läuft oder geht
        bool isRunning = _character != null && _character.MovementState.CurrentState == CharacterStates.MovementStates.Running;

        // Ermittle Untergrund
        string groundType = GetGroundType();

        // Wähle EventReference
        EventReference stepEvent = SelectEvent(groundType, isRunning);

        // Sound abspielen
        if (stepEvent.IsNull) return;

        var instance = RuntimeManager.CreateInstance(stepEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        instance.start();
        instance.release();
    }

    private string GetGroundType()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayerMask);
        if (hit.collider != null)
        {
            return hit.collider.tag; // Boden-Tags wie "Grass", "Stone"
        }
        return "Default";
    }

    private EventReference SelectEvent(string groundType, bool isRunning)
    {
        Debug.Log(groundType);
        switch (groundType)
        {
            case "Untagged":
                return isRunning ? normalWalk : normalRun;
            case "Respawn":
                return isRunning ? lootWalk : lootRun;
            default:
                return isRunning ? normalWalk : normalRun;
        }
    }

    private void OnDestroy()
    {
        if (skeletonAnimation != null)
            skeletonAnimation.AnimationState.Event -= HandleSpineEvent;
    }
}
