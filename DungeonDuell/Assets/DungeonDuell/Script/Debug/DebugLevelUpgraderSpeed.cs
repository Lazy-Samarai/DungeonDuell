using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

namespace dungeonduell
{
    public class DebugLevelUpgraderSpeed : MonoBehaviour // if used more do Innherritace with other debug Uphgraddcer
    {
        [FormerlySerializedAs("UpLevelKey")] [SerializeField]
        private KeyCode upLevelKey;

        [FormerlySerializedAs("DownLevelKey")] [SerializeField]
        private KeyCode downLevelKey;

        private float _baseRun;
        private float _baseWalk;
        private int _level = 1;
        private CharacterRun _running;
        private CharacterMovement _walking;

        private void Start()
        {
            _walking = GetComponent<CharacterMovement>();
            _running = GetComponent<CharacterRun>();
            _baseWalk = _walking.WalkSpeed;
            _baseRun = _running.RunSpeed;
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
            _walking.WalkSpeed = _baseWalk + (_level - 1);
            _walking.MovementSpeed = _baseRun + (_level - 1);

            _running.RunSpeed = _baseRun + (_level - 1);
        }
    }
}