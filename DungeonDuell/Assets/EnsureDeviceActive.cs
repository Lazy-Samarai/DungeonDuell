using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace dungeonduell
{
    public class EnsureDeviceActive : MonoBehaviour // Fail Safe
    {
        void Start()
        {
            foreach (PlayerInput playerInput in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
            {
                try
                {
                    InputSystem.EnableDevice(playerInput.user.pairedDevices[0]);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}