using Unity.VisualScripting;
using UnityEngine;

namespace dungeonduell
{
    public class SceneCurtainOnInput : SceneCurtain
    {
        private readonly bool[] _playerInput = new bool[2];

        protected override void Start()
        {
            // Override to Do Nothing
        }

        public void InputCurtainHandle(bool player)
        {
            if (!_playerInput[player ? 1 : 0])
            {
                _playerInput[player ? 1 : 0] = true;
                ChangeCurtainSingle(player, true);
            }
        }
    }
}