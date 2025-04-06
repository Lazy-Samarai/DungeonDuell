using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    public class WeaponSpineAnimationHandling : MonoBehaviour
    {
        private PlayerSpineAnimationHandling _playerSpineAnimationHandling;
        void Start()
        {
            _playerSpineAnimationHandling = GetComponentInParent<Character>().gameObject.GetComponentInChildren<PlayerSpineAnimationHandling>();
        }

        public void HandleShootAnimation()
        {
            _playerSpineAnimationHandling.SetToShoot();
        }
    }
}
