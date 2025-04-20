using System;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    public class DebugLevelUpgrader : MonoBehaviour
    {
        [SerializeField] private KeyCode UpLevelKey;
        [SerializeField] private KeyCode DownLevelKey;
        private int level = 1;
        private ProjectileWeapon weapon;

        private void Start()
        {
            weapon = GetComponent<ProjectileWeapon>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(UpLevelKey)) ChangeLevel(true);
            if (Input.GetKeyDown(DownLevelKey)) ChangeLevel(false);
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
                if (level <= 0) level = 1;
            }

            AdjustToLevel();
        }

        public void AdjustToLevel()
        {
            weapon.TimeBetweenUses = (float)(1 / Math.Pow(2, level - 1));
        }
    }
}