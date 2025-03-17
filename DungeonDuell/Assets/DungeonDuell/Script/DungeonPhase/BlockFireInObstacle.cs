using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class BlockFireInObstacle : MonoBehaviour
    {
        int colldingCount;
        MoreMountains.TopDownEngine.ProjectileWeapon projectileWeapon;
        void Start()
        {
            projectileWeapon = GetComponent<MoreMountains.TopDownEngine.ProjectileWeapon>();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 8)
            {
                colldingCount++;
                projectileWeapon.InputAuthorized = false;
            }


        }
        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 8)
            {
                colldingCount--;
                if (colldingCount <= 0)
                {
                    projectileWeapon.InputAuthorized = true;
                }

            }

        }
    }
}
