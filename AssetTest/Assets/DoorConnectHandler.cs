using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class DoorConnectHandler : MonoBehaviour
    {
        public int myId { get; set; }
        public GameObject Doors;
        
        public void SetDoorConnectFull(Tuple<DoorConnectHandler, ConnectionDir> roomConnection)
        {
            // To target
            SetDoorConncectSingle(roomConnection);
            
            
            Tuple<DoorConnectHandler, ConnectionDir> ConncectionFromTarget =
            new Tuple<DoorConnectHandler, ConnectionDir>(this, roomConnection.Item2.GetInvert());


            // From Target 
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
        }
    }
}
