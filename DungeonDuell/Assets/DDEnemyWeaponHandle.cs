using System;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    public class DDEnemyWeaponHandle : CharacterHandleWeapon
    {
        [SerializeField] AIBrain aiBrain;
        private const String Player1 = "Player1";
        public MMFeedbacks weaponUseTargetPlayer1;
        public MMFeedbacks weaponUseTargetPlayer2;

        public override void PlayAbilityStartFeedbacks()
        {
            bool player1 = aiBrain.Target.gameObject.GetComponent<Character>().PlayerID == Player1;
            AbilityStartFeedbacks = player1 ? weaponUseTargetPlayer1 : weaponUseTargetPlayer2;
            base.PlayAbilityStartFeedbacks();
        }
    }
}