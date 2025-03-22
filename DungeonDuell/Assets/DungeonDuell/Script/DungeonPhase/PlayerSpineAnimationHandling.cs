using Spine;
using Spine.Unity;
using UnityEngine;

namespace dungeonduell
{
    public class PlayerSpineAnimationHandling : MonoBehaviour
    {
        const string TargetBoneName = "Target"; 
        private Bone _ikTargetBone;
        SkeletonAnimation _skeletonAnimation;
        [SpineAnimation] public string running;
        [SpineAnimation] public string idle;
        [SpineAnimation] public string walk;
        [SpineAnimation] public string death;
        [SpineAnimation] public string shoot;
        [SpineAnimation] public string dash;

        public float runningMultiply = 1f;
        public float walkMultiply = 1f;

        // Start is called before the first frame update
        void Start()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _ikTargetBone = _skeletonAnimation.Skeleton.FindBone(TargetBoneName); 
            SetToIdle();
        }

        private void Update()
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;
            if (transform.parent.transform.rotation.y <= 0)
            {
                mouseWorldPosition.x *= -1;
            }
            
            _ikTargetBone.SetLocalPosition(mouseWorldPosition);
            
            
            _skeletonAnimation.skeleton.UpdateWorldTransform(Skeleton.Physics.Update);
            
        }

        public void SetAnimation(string aniName)
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, aniName, true);
        }

        public void SetAnimation(string aniName, bool loop)
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, aniName, loop);
        }

        public void SetAnimation(string aniName, float scale)
        {
            TrackEntry trackEntry = _skeletonAnimation.AnimationState.SetAnimation(0, aniName, true);
            trackEntry.TimeScale = scale;
        }

        // Done like this to avoid String reference
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
        public void SetToDeath()
        {
            print("Deeeeeeeeeeeaddddd");
            SetAnimation(death, false);
        }
        public void SetToDash()
        {
            SetAnimation(dash);
        }

        public void SetToShoot()
        {
            _skeletonAnimation.AnimationState.AddAnimation(1,shoot,false,0f);
            _skeletonAnimation.AnimationState.AddEmptyAnimation(1, 0.25f, 0f);
        }

     
    }
}