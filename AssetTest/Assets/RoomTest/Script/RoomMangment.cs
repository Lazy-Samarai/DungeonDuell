using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

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
        GameObject StartRoomPlayer_1;

        [SerializeField] Transform spawnPoint_Player2;
        GameObject StartRoomPlayer_2; 

        void Awake()
        {
            // Conncection will always be two sided, 
            RoomsInfos = FindAnyObjectByType<ConnectionsCollector>().GetRoomList();

            RoomsInfosWithPos = FindAnyObjectByType<ConnectionsCollector>().GetFullRoomList();
            
            // Convert Both Side Connect | 
            foreach (Tuple<Vector3Int, RoomInfo> roomInfo in RoomsInfosWithPos)
            {
                foreach (RoomConnection roomConnection in roomInfo.Item2.Conncection)
                {
                   RoomsInfosWithPos.FirstOrDefault(obj => obj.Item2.RoomID == roomConnection.targetRoomId)
                        .Item2.Conncection.Add(
                       new RoomConnection(roomInfo.Item2.RoomID, roomConnection.connectionDir.GetInvert()));

                    
                }
            }

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

                RoomPortHandler roomPortHandler = Nextroom.GetComponentInChildren<RoomPortHandler>();
                
                foreach (RoomConnection rc in roomInfo.Item2.Conncection)
                {
                    roomPortHandler.OpenPort(rc.connectionDir);
                }


                DoorConnectHandler roomdoorHandler = Nextroom.GetComponent<DoorConnectHandler>();
                roomdoorHandler.myId = roomInfo.Item2.RoomID;
                DoorCollect.Add(roomdoorHandler);

                if(roomInfo.Item2.roomtype == RoomType.Spawn_Player1)
                {
                    StartRoomPlayer_1 = Nextroom;
                    // Some Player Check required later for Mutiplayer here 
                    spawnPoint_Player1.transform.position = new Vector3(posX, posY, 0);
                   // Nextroom.GetComponentInChildren<CinemachineVirtualCamera>(true).gameObject.SetActive(false); // force update
                    // Nextroom.GetComponentInChildren<CinemachineVirtualCamera>(true).gameObject.SetActive(true);

                }
                if(roomInfo.Item2.roomtype == RoomType.Spawn_Player2)
                {
                    StartRoomPlayer_2 = Nextroom;             
                    spawnPoint_Player2.transform.position = new Vector3(posX, posY, 0);                    
                }

            }
            Destroy(transform.GetChild(0).gameObject);

            StartRoomPlayer_1.GetComponentInChildren<CinemachineVirtualCamera>(true).gameObject.SetActive(false); // force update
            StartRoomPlayer_1.GetComponentInChildren<CinemachineVirtualCamera>(true).gameObject.SetActive(true);

            // Destroying Prefab in Scene. Prefab in Szene because it caused porblem with virtsula Cam. Curently virtsula Cam are not on by
            // room to room basis but this will stay like this until a new Cam handling is implented
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
