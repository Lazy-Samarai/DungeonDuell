using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine;

namespace dungeonduell
{
    public class MinimapFollow : MonoBehaviour
    {
        [SerializeField] private bool isPlayer1 = true;
        private Transform target;

        void LateUpdate()
        {
            if (target != null)
                transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }

        public void SetFollowTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public bool MatchesPlayerTag(string tag)
        {
            return isPlayer1 ? tag == "Player1" : tag == "Player2";
        }
    }
}