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
        [SpineAnimation]
        public string idle;
        [SpineAnimation]
        public string walk;

        // Start is called before the first frame update
        void Start()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            SetToIdle();
        
        }
        public void SetAnimation(string name)
        {
            TrackEntry trackEntry =  skeletonAnimation.AnimationState.SetAnimation(0, name, true);
            trackEntry.TimeScale = 1;
        }
        public void SetToIdle()
        {
            SetAnimation(idle);
        }
        public void SetToRunning()
        {
            SetAnimation(running);
        }
        public void SetToWalk()
        {
            SetAnimation(walk);
        }



    }
}
