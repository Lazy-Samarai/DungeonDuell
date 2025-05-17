using Spine;
using Spine.Unity;
using UnityEngine;

namespace dungeonduell
{
    public class PlayerSpineAnimationHandling : SpineCharacterAnimationHandler
    {
        private const string TargetBoneName = "Target";
        [SpineAnimation] public string running;
        [SpineAnimation] public string runningBackward;

        [SpineAnimation] public string dash;
        [SpineAnimation] public string dashReverse;

        public float runningMultiply = 1f;
        public float walkMultiply = 1f;

        private Bone _ikTargetBone;

        public ParticleSystemRenderer dashParticlesRenderer;
        public Material dashParticlesMaterial;
        public Material dashParticlesBackwardsMaterial;

        protected override void Awake()
        {
            base.Awake();
            //   _ikTargetBone = _skeletonAnimation.Skeleton.FindBone(TargetBoneName);
            SetToIdle();
        }

        private void Update()
        {
            if (Camera.main != null)
            {
                var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0;

                if (transform.parent.transform.rotation.y <= 0) mouseWorldPosition.x *= -1;
            }

            CheckRunningDirection();

//            _ikTargetBone.SetLocalPosition(mouseWorldPosition);

            SkeletonAnimation.skeleton.UpdateWorldTransform(Skeleton.Physics.Update);
        }

        protected override void UpdateFacing()
        {
            dashParticlesRenderer.flip = new Vector3(facingEastRunning ? 1 : 0, 0, 0);

            if ((SkeletonAnimation.state.GetCurrent(0).ToString() == running) |
                (SkeletonAnimation.state.GetCurrent(0).ToString() == runningBackward)) SetToRunning();
        }

        public void SetToRunning()
        {
            var backwards = facingEastRunning != runningEast;
            SetAnimation(!backwards ? running : runningBackward, runningMultiply);
        }

        public override void SetToBaseMovement()
        {
            SetAnimation(baseMoving, walkMultiply);
        }

        public void SetToDash()
        {
            var backwards = facingEastRunning != runningEast;
            dashParticlesRenderer.material = backwards ? dashParticlesBackwardsMaterial : dashParticlesMaterial;
            SetAnimation(!backwards ? dash : dashReverse);
        }

        public override void SetToAttacking()
        {
            SkeletonAnimation.AnimationState.AddAnimation(1, attack, false, 0f);
            SkeletonAnimation.AnimationState.AddEmptyAnimation(1, 0.25f, 0f);
        }

        public void SetSkin(int indexOfSkin)
        {
            SkeletonAnimation.skeleton.SetSkin(SkeletonAnimation.skeleton.Data.Skins.Items[indexOfSkin]);
            SkeletonAnimation.skeleton.SetSlotsToSetupPose();
        }
    }
}