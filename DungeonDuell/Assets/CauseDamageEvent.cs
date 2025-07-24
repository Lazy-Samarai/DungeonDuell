using UnityEngine;

namespace dungeonduell
{
    public class CauseDamageEvent : MonoBehaviour
    {
        public void CausePlayerGotDamagedEvent(bool player1)
        {
            DdCodeEventHandler.Trigger_PlayerGotDamaged(player1);
        }
    }
}