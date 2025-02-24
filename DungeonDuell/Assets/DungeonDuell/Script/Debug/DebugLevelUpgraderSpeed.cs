using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace dungeonduell
{
    public class DebugLevelUpgraderSpeed : MonoBehaviour // if used more do Innherritace with other debug Uphgraddcer
    {
        int level = 1;
        CharacterMovement walking;
        CharacterRun running;
        [SerializeField] KeyCode UpLevelKey;
        [SerializeField] KeyCode DownLevelKey;
        float baseWalk;
        float baseRun;

        private void Start()
        {
            walking = GetComponent<CharacterMovement>();
            running = GetComponent<CharacterRun>();
            baseWalk = walking.WalkSpeed;
            baseRun = running.RunSpeed;
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
                if (level <= 0)
                {
                    level = 1;
                }
            }
            AdjustToLevel();
        }
        public void AdjustToLevel()
        {
            walking.WalkSpeed = baseWalk + (level - 1);
            walking.MovementSpeed = baseRun + (level - 1);
           
            running.RunSpeed = baseRun + (level - 1);
           
        }

    }
}

