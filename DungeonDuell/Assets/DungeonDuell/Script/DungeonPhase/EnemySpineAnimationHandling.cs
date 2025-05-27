using Spine;
using Spine.Unity;
using UnityEngine;

namespace dungeonduell
{
    public class EnemySpineAnimationHandling : SpineCharacterAnimationHandler
    {
        [SpineAnimation] public string damage;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Awake()
        {
            base.Awake();
            SetToIdle();
        }

        public override void SetToAttacking()
        {
            SkeletonAnimation.AnimationState.AddAnimation(1, attack, false, 0f);
            SkeletonAnimation.AnimationState.AddEmptyAnimation(1, 0.25f, 0f);
        }

        public void SetToDamage()
        {
            SkeletonAnimation.AnimationState.AddAnimation(1, damage, false, 0f);
            SkeletonAnimation.AnimationState.AddEmptyAnimation(1, 0.25f, 0.5f);
        }
    }
}