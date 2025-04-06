using UnityEngine;

namespace dungeonduell
{
    public class DeviceInitializer : MonoBehaviour
    {
        void Start()
        {
            DeviceHelper.SetupDevicesManuell();
        }
    }
}
