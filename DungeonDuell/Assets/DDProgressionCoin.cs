using System;
using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    public class DDProgressionCoin : ProgressionCoin
    {
        public MMFeedbacks PickedFeedbacksPlayer1;
        public MMFeedbacks PickedFeedbacksPlayer2;
        private const String Player1 = "Player1";

        public override void PickItem(GameObject picker)
        {
            var character = picker.GetComponent<Character>();
            bool player1 = character.PlayerID == Player1;
            PickedMMFeedbacks = player1 ? PickedFeedbacksPlayer1 : PickedFeedbacksPlayer2;
            base.PickItem(picker);
        }
    }
}