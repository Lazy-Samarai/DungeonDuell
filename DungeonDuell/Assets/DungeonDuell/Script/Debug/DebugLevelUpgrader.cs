using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class DebugLevelUpgrader : MonoBehaviour
    {
        int level = 1;
        ProjectileWeapon weapon;
        [SerializeField] KeyCode UpLevelKey;
        [SerializeField] KeyCode DownLevelKey;

        private void Start()
        {
            weapon = GetComponent<ProjectileWeapon>();
        }
        private void Update()
        {
            if (Input.GetKeyDown(UpLevelKey))
            {
                ChangeLevel(true);
            }
            if (Input.GetKeyDown(DownLevelKey))
            {
                ChangeLevel(false);
            }
        }

        public void ChangeLevel(bool up)
        {
            if (up)
            {
                level++;
            }
            else
            {
                level--;
                if(level <= 0)
                {
                    level = 1;
                }
            }
            AdjustToLevel();
        }
        public void AdjustToLevel()
        {
            weapon.TimeBetweenUses = (float)(1 / (Math.Pow(2, (level - 1))));
        }

    }
}
