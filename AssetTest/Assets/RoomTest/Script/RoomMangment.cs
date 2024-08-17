using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class RoomMangment : MonoBehaviour
    {
        public List<RoomInfo> RoomsInfos = new List<RoomInfo>();
        public List<Tuple<Vector3Int, RoomInfo>> RoomsInfosWithPos = new List<Tuple<Vector3Int, RoomInfo>>();

        List<DoorConnectHandler> DoorCollect = new List<DoorConnectHandler>();
        [SerializeField] GameObject roomprefab;

        [SerializeField] float stepCross;
        [SerializeField] float stepUpDown;

        [SerializeField] Transform spawnPoint_Player1;
        [SerializeField] Transform spawnPoint_Player2; // Empty Until Mutiplayer is implented 

        void Awake()
        {
            // Conncection will always be two sided, 
            RoomsInfos = FindAnyObjectByType<ConnectionsCollector>().GetRoomList();

            RoomsInfosWithPos = FindAnyObjectByType<ConnectionsCollector>().GetFullRoomList();
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

            foreach (Tuple<Vector3Int, RoomInfo> roomInfo in RoomsInfosWithPos) // spawing all rooms in 
            {
                GameObject Nextroom = Instantiate(roomprefab, transform);

                // odd and even y pos diff in a Hex Grid  
                float posX = (roomInfo.Item1.y % 2 == 0) ? roomInfo.Item1.x * stepCross : stepCross / 2 + roomInfo.Item1.x * stepCross;
                float posY = roomInfo.Item1.y * stepUpDown;

                Nextroom.transform.localPosition = new Vector3(posX, posY, 0);

                Nextroom.GetComponentInChildren<InteriorSpawner>().SpawnInterior(roomInfo.Item2.roomtype);

                DoorConnectHandler roomdoorHandler = Nextroom.GetComponent<DoorConnectHandler>();
                roomdoorHandler.myId = roomInfo.Item2.RoomID;
                DoorCollect.Add(roomdoorHandler);

                if(roomInfo.Item2.roomtype == RoomType.Spawn_Player1)
                {
                    // Some Player Check required later for Mutiplayer here 
                    spawnPoint_Player1.transform.position = new Vector3(posX, posY, 0);
                }

            }
            

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
