using MoreMountains.Tools;
using UnityEngine;

namespace dungeonduell
{
    public class StopOnWinEnemy : StopOnWin
    {
        void OnEnable()
        {
            SubscribeToEvents();
        }
        private void OnDisable()
        {
            UnsubscribeToAllEvents();
        }

        protected override void StoppingOnWin(string _)
        {
            base.StoppingOnWin(_);
            GetComponent<AIBrain>().ResetBrain();
            GetComponent<AIBrain>().enabled = false;
            GetComponentInChildren<EnemySpineAnimationHandling>().SetToIdle();
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
        
    }
}
