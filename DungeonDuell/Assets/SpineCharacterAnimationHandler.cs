using System;
using DG.Tweening;
using MoreMountains.TopDownEngine;
using Spine;
using Spine.Unity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace dungeonduell
{
    public class SpineCharacterAnimationHandler : MonoBehaviour
    {
        [SpineAnimation] public string idle;
        [SpineAnimation] public string walk;

        public bool facingEastRunning;
        public bool runningEast;

        private CharacterMovement _characterMovement;

        private SkeletonAnimation _skeletonAnimation;

        private void Start()
        {
            _characterMovement = GetComponentInParent<CharacterMovement>();
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
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

        private void UpdateFacing()
        {
        }

        private void SetAnimation(string aniName)
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, aniName, true);
        }

        private void SetAnimation(string aniName, bool loop)
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, aniName, loop);
        }
    }
}