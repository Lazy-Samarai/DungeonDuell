using System;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

namespace dungeonduell
{
    public class DebugLevelUpgrader : MonoBehaviour
    {
        [FormerlySerializedAs("UpLevelKey")] [SerializeField]
        private KeyCode upLevelKey;

        [FormerlySerializedAs("DownLevelKey")] [SerializeField]
        private KeyCode downLevelKey;

        private int _level = 1;
        private ProjectileWeapon _weapon;

        private void Start()
        {
            _weapon = GetComponent<ProjectileWeapon>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(upLevelKey)) ChangeLevel(true);
            if (Input.GetKeyDown(downLevelKey)) ChangeLevel(false);
        }

        public void ChangeLevel(bool up)
        {
            if (up)
            {
                _level++;
            }
            else
            {
                _level--;
                if (_level <= 0) _level = 1;
            }

            AdjustToLevel();
        }

        public void AdjustToLevel()
        {
            _weapon.TimeBetweenUses = (float)(1 / Math.Pow(2, _level - 1));
        }
    }
}