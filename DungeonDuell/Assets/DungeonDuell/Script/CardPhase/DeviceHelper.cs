using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using System.Linq;
using MoreMountains.TopDownEngine;

namespace dungeonduell
{
    public static class DeviceHelper
    {
        public static void SetupDevicesManuell()
        {
            var gamepads = Gamepad.all;
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;

            foreach (var playerInput in Object.FindObjectsOfType<PlayerInput>())
            {
                var manager = playerInput.GetComponent<InputSystemManagerEventsBased>();
                if (manager == null) continue;

                playerInput.user.UnpairDevices();

                if (manager.PlayerID == "Player1")
                {
                    if (gamepads.Count >= 2)
                    {
                        InputUser.PerformPairingWithDevice(gamepads[0], playerInput.user);
                        playerInput.SwitchCurrentControlScheme("Gamepad", gamepads[0]);
                    }
                    else
                    {
                        InputUser.PerformPairingWithDevice(keyboard, playerInput.user);
                        InputUser.PerformPairingWithDevice(mouse, playerInput.user);
                        playerInput.SwitchCurrentControlScheme("Keyboard", keyboard, mouse);
                    }
                }
                else if (manager.PlayerID == "Player2")
                {
                    if (gamepads.Count >= 2)
                    {
                        InputUser.PerformPairingWithDevice(gamepads[1], playerInput.user);
                        playerInput.SwitchCurrentControlScheme("Gamepad", gamepads[1]);
                    }
                    else if (gamepads.Count == 1)
                    {
                        InputUser.PerformPairingWithDevice(gamepads[0], playerInput.user);
                        playerInput.SwitchCurrentControlScheme("Gamepad", gamepads[0]);
                    }
                }

                Debug.Log($"[DeviceHelper] {manager.PlayerID} uses: {string.Join(", ", playerInput.user.pairedDevices.Select(d => d.displayName))}");
            }
        }
    }
}
