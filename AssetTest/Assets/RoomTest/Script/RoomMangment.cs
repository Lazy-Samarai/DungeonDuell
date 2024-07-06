using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class RoomMangment : MonoBehaviour
    {
        public List<RoomInfo> RoomsInfos = new List<RoomInfo>();
        List<DoorConnectHandler> DoorCollect = new List<DoorConnectHandler>();
        [SerializeField] GameObject roomprefab;
        void Start()
        {
            // Conncection will always be two sided, 
            RoomsInfos = FindAnyObjectByType<ConnectionsCollector>().GetRoomList();

            /*
            RoomsInfos.Add(
                new RoomInfo(0,
                new List<RoomConnection>
                {new RoomConnection(1, ConnectionDir.TopRight),
                 new RoomConnection(2, ConnectionDir.Right)}));
            RoomsInfos.Add(
                new RoomInfo(1,
                new List<RoomConnection>
                {new RoomConnection(3, ConnectionDir.TopLeft)}));
            RoomsInfos.Add(
                new RoomInfo(2,
                new List<RoomConnection>
                {new RoomConnection(5, ConnectionDir.TopRight) }));
            RoomsInfos.Add(
                new RoomInfo(3,
                new List<RoomConnection>
                {new RoomConnection(4, ConnectionDir.TopRight) }));
            RoomsInfos.Add(
                new RoomInfo(4,
                new List<RoomConnection>
                { }));
            RoomsInfos.Add(
                new RoomInfo(5,
                new List<RoomConnection>
                { new RoomConnection(1, ConnectionDir.Left)}));

            */


            // Generate
            GenerateRooms(RoomsInfos);
        }

        private void GenerateRooms(List<RoomInfo> RoomsInfos)
        {

            float xpos = 0;
            foreach (RoomInfo roomInfo in RoomsInfos) // spawing all rooms in 
            {
                GameObject Nextroom = Instantiate(roomprefab, transform);
                Nextroom.transform.localPosition = new Vector3(xpos, 0, 0);

                DoorConnectHandler roomdoorHandler = Nextroom.GetComponent<DoorConnectHandler>();
                roomdoorHandler.myId = roomInfo.RoomID;
                DoorCollect.Add(roomdoorHandler);


                xpos = xpos + 20; // prevent room overlap,
                // as only the current room can be ssen to player
                // it is relvant where the rooms actaully are as long the conncection are right

            }
            foreach (RoomInfo roomInfo in RoomsInfos)
            {
                DoorConnectHandler RoomsConnectHandler = GetByIdDoorHandl(roomInfo.RoomID);
                print(roomInfo.RoomID);
                foreach (RoomConnection roomConnection in roomInfo.Conncection)
                {
                    // refence to Target Room
                    DoorConnectHandler doorTargetConnectHandler = GetByIdDoorHandl(roomConnection.targetRoomId);
                    if (doorTargetConnectHandler != null)
                    {
                        Tuple<DoorConnectHandler, ConnectionDir> doorInfo =
                            new Tuple<DoorConnectHandler, ConnectionDir>(doorTargetConnectHandler, roomConnection.connectionDir);

                        print("for Room " + roomInfo.RoomID + "Conncect to " + roomConnection.targetRoomId);

                        RoomsConnectHandler.SetDoorConnectFull(doorInfo);


                    }
                }

            }
            DeativateUnsedDoors();
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            // for some Reason an Room already in the scene as to be used as Prefab otherwiese
            // the virtual cam get scuffed
            // part of the InilizeRoom are deactivate here
        }

        private void DeativateUnsedDoors()
        {
            foreach (DoorConnectHandler handler in DoorCollect)
            {
                handler.DeactivateUnusedDoor();
            }
        }

        public DoorConnectHandler GetByIdDoorHandl(int id)
        {
            foreach(DoorConnectHandler doorConnect in DoorCollect)
            {
                if(doorConnect.myId == id)
                {
                    return doorConnect;
                }
            }
            return null;
        }

    }
}
