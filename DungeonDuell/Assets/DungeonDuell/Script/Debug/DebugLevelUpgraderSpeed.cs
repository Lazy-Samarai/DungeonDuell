using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    public class DebugLevelUpgraderSpeed : MonoBehaviour // if used more do Innherritace with other debug Uphgraddcer
    {
        [SerializeField] private KeyCode UpLevelKey;
        [SerializeField] private KeyCode DownLevelKey;
        private float baseRun;
        private float baseWalk;
        private int level = 1;
        private CharacterRun running;
        private CharacterMovement walking;

        private void Start()
        {
            walking = GetComponent<CharacterMovement>();
            running = GetComponent<CharacterRun>();
            baseWalk = walking.WalkSpeed;
            baseRun = running.RunSpeed;
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
            walking.WalkSpeed = baseWalk + (level - 1);
            walking.MovementSpeed = baseRun + (level - 1);

            running.RunSpeed = baseRun + (level - 1);
        }
    }
}