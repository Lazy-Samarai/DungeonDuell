using UnityEngine;

namespace dungeonduell
{
    public class MiniMapRoomPortHandler : RoomPortHandler
    {
        public void SetMapInvisble()
        {
            gameObject.SetActive(false);
        }

        public override void OpenPort(ConnectionDir dir)
        {
            // Simplyfication not really need but well. IF fruther dev make this base and RoomPortHandler get this. 
            GetPort(dir).SetActive(false);
        }
    }
}