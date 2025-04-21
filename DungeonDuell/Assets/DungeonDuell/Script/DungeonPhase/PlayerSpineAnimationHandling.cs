using MoreMountains.TopDownEngine;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace dungeonduell
{
    public class PlayerSpineAnimationHandling : MonoBehaviour
    {
        const string TargetBoneName = "Target";
        private Bone _ikTargetBone;
        SkeletonAnimation _skeletonAnimation;
        [SpineAnimation] public string running;
        [SpineAnimation] public string runningBackward;
        [SpineAnimation] public string idle;
        [SpineAnimation] public string walk;
        [SpineAnimation] public string death;
        [SpineAnimation] public string shoot;
        [SpineAnimation] public string dash;

        public float runningMultiply = 1f;
        public float walkMultiply = 1f;

        public bool facingEastRunning = false;
        public bool runningEast = false;
        [FormerlySerializedAs("_rigidbody2D")] public CharacterMovement _characterMovement;


        // Start is called before the first frame update
        void Awake()
        {
            _characterMovement = GetComponentInParent<CharacterMovement>();
            //   _characterOrientation2D  = GetComponentInParent<CharacterOrientation2D>();
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

            CheckRunningDirection();

            _ikTargetBone.SetLocalPosition(mouseWorldPosition);


            _skeletonAnimation.skeleton.UpdateWorldTransform(Skeleton.Physics.Update);
        }

        private void CheckRunningDirection()
        {
            if (transform.parent.transform.rotation.y <= 0 != facingEastRunning)
            {
                facingEastRunning = transform.parent.transform.rotation.y <= 0;
                UpdateFacing();
            }

            if (_characterMovement.GetMovement().x >= 0 != runningEast)
            {
                runningEast = _characterMovement.GetMovement().x >= 0;
                UpdateFacing();
            }
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

        public void UpdateFacing()
        {
            if (_skeletonAnimation.state.GetCurrent(0).ToString() == running |
                _skeletonAnimation.state.GetCurrent(0).ToString() == runningBackward)
            {
                SetToRunning();
            }
        }

        // Done like this to avoid String reference
        public void SetToIdle()
        {
            SetAnimation(idle);
        }

        public void SetToRunning()
        {
            bool backwards = facingEastRunning != runningEast;
            SetAnimation(!backwards ? running : runningBackward, runningMultiply);
        }

        public void SetToWalk()
        {
            SetAnimation(walk, walkMultiply);
        }

        public void SetToDeath()
        {
            SetAnimation(death, false);
        }

        public void SetToDash()
        {
            SetAnimation(dash);
        }

        public void SetToShoot()
        {
            _skeletonAnimation.AnimationState.AddAnimation(1, shoot, false, 0f);
            _skeletonAnimation.AnimationState.AddEmptyAnimation(1, 0.25f, 0f);
        }
    }
}