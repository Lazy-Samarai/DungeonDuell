using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace dungeonduell
{
    public class PlayerSpineAnimationHandling : MonoBehaviour
    {
        SkeletonAnimation skeletonAnimation;
        [SpineAnimation]
        public string running;
        public float runningMultiply = 1f;
        [SpineAnimation]
        public string idle;
        [SpineAnimation]
        public string walk;
        public float walkMultiply = 1f;

        // Start is called before the first frame update
        void Start()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            SetToIdle();

        }
        public void SetAnimation(string name)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, name, true);
        }
        public void SetAnimation(string name, float scale)
        {
            TrackEntry trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, name, true);
            trackEntry.TimeScale = scale;
        }
        public void SetToIdle()
        {
            SetAnimation(idle);
        }
        public void SetToRunning()
        {
            SetAnimation(running, runningMultiply);
        }
        public void SetToWalk()
        {
            SetAnimation(walk, walkMultiply);
        }
    }
}
