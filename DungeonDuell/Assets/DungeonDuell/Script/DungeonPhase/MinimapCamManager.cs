using UnityEngine;

namespace dungeonduell
{
    public class MinimapCamManager : MonoBehaviour
    {
        [SerializeField] private MinimapFollow minimapFollow_Player1;
        [SerializeField] private MinimapFollow minimapFollow_Player2;
        
        public void SetFollowTarget(Transform target, bool isPlayer1)
        {
            if (isPlayer1)
                minimapFollow_Player1.SetFollowTarget(target);
            else
                minimapFollow_Player2.SetFollowTarget(target);
        }
    }
}