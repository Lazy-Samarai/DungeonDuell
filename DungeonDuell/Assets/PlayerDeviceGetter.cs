using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace dungeonduell
{
    public class PlayerDeviceGetter : MonoBehaviour
    {
        [SerializeField] PlayerInput[] playerInputs;

        public bool CheckIfPlayerHasGamepad(int playerID)
        {
            InputDevice inputDevice = playerInputs[playerID].user
                .pairedDevices[0];
            if (inputDevice is Gamepad)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}