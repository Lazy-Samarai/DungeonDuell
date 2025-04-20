using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    public class BlockFireInObstacle : MonoBehaviour
    {
        private int colldingCount;
        private ProjectileWeapon projectileWeapon;

        private void Start()
        {
            projectileWeapon = GetComponent<ProjectileWeapon>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 8)
            {
                colldingCount++;
                projectileWeapon.InputAuthorized = false;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 8)
            {
                colldingCount--;
                if (colldingCount <= 0) projectileWeapon.InputAuthorized = true;
            }
        }
    }
}