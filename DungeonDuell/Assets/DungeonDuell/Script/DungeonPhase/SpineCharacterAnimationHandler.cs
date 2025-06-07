using MoreMountains.TopDownEngine;
using Spine.Unity;
using UnityEngine;

namespace dungeonduell
{
    public class SpineCharacterAnimationHandler : MonoBehaviour
    {
        public bool facingEastRunning;
        public bool runningEast;

        [SpineAnimation] public string idle;
        [SpineAnimation] public string death;
        [SpineAnimation] public string baseMoving;
        [SpineAnimation] public string attack;

        private CharacterMovement _characterMovement;

        protected SkeletonAnimation SkeletonAnimation;

        protected virtual void Awake()
        {
            _characterMovement = GetComponentInParent<CharacterMovement>();
            SkeletonAnimation = GetComponent<SkeletonAnimation>();
        }

        protected void CheckRunningDirection()
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

        protected virtual void UpdateFacing()
        {
        }

        // Done like this to avoid String reference
        public void SetToIdle()
        {
            SetAnimation(idle);
        }

        public void SetToDeath()
        {
            SetAnimation(death, false);
        }

        public virtual void SetToBaseMovement()
        {
            SetAnimation(baseMoving);
        }

        public virtual void SetToAttacking()
        {
            SetAnimation(attack);
        }

        protected void SetAnimation(string aniName)
        {
            SkeletonAnimation.AnimationState.SetAnimation(0, aniName, true);
        }

        protected void SetAnimation(string aniName, bool loop)
        {
            SkeletonAnimation.AnimationState.SetAnimation(0, aniName, loop);
        }

        protected void SetAnimation(string aniName, float scale)
        {
            var trackEntry = SkeletonAnimation.AnimationState.SetAnimation(0, aniName, true);
            trackEntry.TimeScale = scale;
        }
    }
}