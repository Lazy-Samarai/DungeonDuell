using TMPro;
using UnityEngine;

namespace dungeonduell
{
    public class GetDevicesExample : MonoBehaviour
    {
        public int playerID;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GetComponent<TextMeshPro>().text =
                FindFirstObjectByType<PlayerDeviceGetter>().CheckIfPlayerHasGamepad(playerID)
                    ? "Is Gamepad"
                    : "Is Keyboard";
        }
    }
}