using UnityEngine;
using Spine.Unity;
using FMODUnity;
using FMOD.Studio;
using MoreMountains.TopDownEngine;

public class SpineStepSound : MonoBehaviour
{
    [SpineEvent] public string spineEventName = "Step";

    [Header("FMOD Events")] public EventReference normalWalk;
    public EventReference normalRun;
    public EventReference lootWalk;
    public EventReference lootRun;

    public float hearDistance = 12f;

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

        // Kein Sound, wenn kein Hörer in der Nähe ist
        if (!IsPlayerInRange()) return;

        bool isRunning = _character != null && _character.MovementState.CurrentState == CharacterStates.MovementStates.Running;
        string groundType = GetGroundType();
        EventReference stepEvent = SelectEvent(groundType, isRunning);

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
        switch (groundType)
        {
            case "Untagged":
                return isRunning ? normalWalk : normalRun;
            case "LootRoom":
                return isRunning ? lootWalk : lootRun;
            default:
                return isRunning ? normalWalk : normalRun;
        }
    }

    private bool IsPlayerInRange()
    {
        GameObject[] players1 = GameObject.FindGameObjectsWithTag("Player1");
        GameObject[] players2 = GameObject.FindGameObjectsWithTag("Player2");

        foreach (GameObject player in players1)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= hearDistance)
                return true;
        }

        foreach (GameObject player in players2)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= hearDistance)
                return true;
        }

        return false;
    }

    private void OnDestroy()
    {
        if (skeletonAnimation != null)
            skeletonAnimation.AnimationState.Event -= HandleSpineEvent;
    }
}