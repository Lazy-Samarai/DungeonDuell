using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    public class BlockFireInObstacle : MonoBehaviour
    {
        private int _colldingCount;
        private ProjectileWeapon _projectileWeapon;

        private void Start()
        {
            _projectileWeapon = GetComponent<ProjectileWeapon>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 8)
            {
                _colldingCount++;
                _projectileWeapon.InputAuthorized = false;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 8)
            {
                _colldingCount--;
                if (_colldingCount <= 0) _projectileWeapon.InputAuthorized = true;
            }
        }
    }
}