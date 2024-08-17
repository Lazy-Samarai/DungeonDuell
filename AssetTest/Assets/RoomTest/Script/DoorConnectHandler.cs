using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class DoorConnectHandler : MonoBehaviour
    {
        // Door as System ist not used anymore (fully concted rooms) but i decied to keep this code until further noticed. To fully get back the system lock what was removed in corresponding commit in RoomMangment. 

        public int myId { get; set; }
        public GameObject Doors;
        public HashSet<ConnectionDir> usedDoors = new HashSet<ConnectionDir>();
        
        public void SetDoorConnectFull(Tuple<DoorConnectHandler, ConnectionDir> roomConnection)
        {
            // To target
            SetDoorConncectSingle(roomConnection);
            // From Target 
            Tuple<DoorConnectHandler, ConnectionDir> ConncectionFromTarget =
            new Tuple<DoorConnectHandler, ConnectionDir>(this, roomConnection.Item2.GetInvert());
            roomConnection.Item1.SetDoorConncectSingle(ConncectionFromTarget);

        }

        private void SetDoorConncectSingle(Tuple<DoorConnectHandler, ConnectionDir> roomConnection)
        {
            DoorConnectHandler doorsTarget = roomConnection.Item1;
            MoreMountains.TopDownEngine.Teleporter teleport = Doors.transform.GetChild((int)roomConnection.Item2).GetComponentInChildren<MoreMountains.TopDownEngine.Teleporter>();
            
            teleport.CurrentRoom = GetComponent<MoreMountains.TopDownEngine.Room>();
            
            teleport.TargetRoom = doorsTarget.GetComponent<MoreMountains.TopDownEngine.Room>();

            int targetDoorIndex = (int)((roomConnection.Item2).GetInvert());
            teleport.Destination = doorsTarget.Doors.transform.GetChild(targetDoorIndex).GetComponentInChildren<MoreMountains.TopDownEngine.Teleporter>();
            usedDoors.Add(roomConnection.Item2);
        }
        public void DeactivateUnusedDoor()
        {
            foreach(ConnectionDir door in ConnectionDir.GetValues(typeof(ConnectionDir)))
            {
                if (!usedDoors.Contains(door))
                {
                    Doors.transform.GetChild((int)door).gameObject.SetActive(false);
                }
                
            }
           
        }


    }
}
