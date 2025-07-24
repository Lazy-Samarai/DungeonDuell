using System;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace dungeonduell
{
    public class DDMeleeWeapon : MeleeWeapon
    {
        [MMInspectorGroup("MutiplayerFeedback", true, 23)]
        public MMFeedbacks weaponUseTargetPlayer1;

        public MMFeedbacks weaponUseTargetPlayer2;
        private const String Player1 = "Player1";
        AIBrain aiBrain;

        void Awake()
        {
            aiBrain = GetComponentInParent<AIBrain>();
        }

        public override void WeaponUse()
        {
            bool player1 = aiBrain.Target.gameObject.GetComponent<Character>().PlayerID != Player1;
            WeaponUsedMMFeedback = player1 ? weaponUseTargetPlayer1 : weaponUseTargetPlayer2;
            base.WeaponUse();
        }
    }
}