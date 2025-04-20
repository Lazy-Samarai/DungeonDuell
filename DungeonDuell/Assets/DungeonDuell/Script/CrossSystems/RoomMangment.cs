using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace dungeonduell
{
    public class RoomMangment : MonoBehaviour
    {
        [SerializeField] private List<GameObject> roomPrefabs;
        [SerializeField] private float stepCross;
        [SerializeField] private float stepUpDown;
        [SerializeField] private Transform spawnPoint_Player1;
        [SerializeField] private Transform spawnPoint_Player2;
        public List<Tuple<Vector3Int, RoomInfo>> RoomsInfosWithPos = new();

        private void Awake()
        {
            RoomsInfosWithPos = FindAnyObjectByType<ConnectionsCollector>().GetFullRoomList();

            // Convert Both Side Connect | 
            foreach (var roomInfo in RoomsInfosWithPos)
            foreach (var roomConnection in roomInfo.Item2.Conncection)
                RoomsInfosWithPos.FirstOrDefault(obj => obj.Item2.RoomID == roomConnection.targetRoomId)
                    .Item2.Conncection.Add(
                        new RoomConnection(roomInfo.Item2.RoomID, roomConnection.connectionDir.GetInvert()));

            // Generate
            GenerateRooms();
        }

        private void GenerateRooms()
        {
            foreach (var roomInfo in RoomsInfosWithPos) // spawing all rooms in 
            {
                var roomPrefabToUse = GetRoomPrefabByType(roomInfo.Item2.roomtype);

                var Nextroom = Instantiate(roomPrefabToUse ?? roomPrefabs[0], transform);

                if (roomInfo.Item2.FirstTimeSpawn)
                {
                    roomInfo.Item2.FirstTimeSpawn = false;
                    Nextroom.GetComponentInChildren<RewardSpawner>()?.SpawnReward();
                }

                // odd and even y pos diff in a Hex Grid  
                var posX = roomInfo.Item1.y % 2 == 0
                    ? roomInfo.Item1.x * stepCross
                    : stepCross / 2 + roomInfo.Item1.x * stepCross;
                var posY = roomInfo.Item1.y * stepUpDown;

                Nextroom.transform.localPosition = new Vector3(posX, posY, 0);

                var roomPortHandler = Nextroom.GetComponentInChildren<RoomPortHandler>();

                foreach (var rc in roomInfo.Item2.Conncection) roomPortHandler.OpenPort(rc.connectionDir);

                if (roomInfo.Item2.roomtype == RoomType.Spawn_Player1)
                    // Some Player Check required later for Mutiplayer here 
                    spawnPoint_Player1.transform.position = new Vector3(posX, posY, 0);
                if (roomInfo.Item2.roomtype == RoomType.Spawn_Player2)
                    spawnPoint_Player2.transform.position = new Vector3(posX, posY, 0);
            }

            Destroy(transform.GetChild(0).gameObject);

            // If wered decide to have no speical virtsul Cam Work when remove this old commend: 

            // Destroying Prefab in Scene. Prefab in Szene because it caused porblem with virtsula Cam. Curently virtsula Cam are not on by
            // room to room basis but this will stay like this until a new Cam handling is implented
            // the virtual cam get scuffed
            // part of the InilizeRoom are deactivate here
        }

        // Funktion zum Abrufen des Prefabs basierend auf dem Raumtyp
        private GameObject GetRoomPrefabByType(RoomType roomType)
        {
            switch (roomType)
            {
                case RoomType.Generic:
                    return roomPrefabs[0];
                case RoomType.Spawn_Player1:
                    return roomPrefabs[1];
                case RoomType.Spawn_Player2:
                    return roomPrefabs[2];
                case RoomType.NormalLott:
                    return roomPrefabs[3];
                case RoomType.PreSetLoot:
                    return roomPrefabs[4];
                case RoomType.Enemy:
                    return roomPrefabs[5];
                default:
                    return roomPrefabs[0];
            }
        }
    }
}