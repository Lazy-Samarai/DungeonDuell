using UnityEngine;

namespace dungeonduell
{
    public class DeviceInitializer : MonoBehaviour
    {
        private void Start()
        {
            DeviceHelper.SetupDevicesManuell();
        }
    }
}