using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace dungeonduell
{
    public class ConnectionsCollector : MonoBehaviour
    {    
        public List<Tuple<Vector3Int, RoomInfo>> roomsInfos = new List<Tuple<Vector3Int, RoomInfo>>();

        void Awake()
        {
            ConnectionsCollector[] objs = FindObjectsOfType<ConnectionsCollector>();

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        public void AddRoom(Vector3Int pos, List<RoomConnection> Conncection,RoomType type, RoomElement element)
        {
            Tuple<Vector3Int, RoomInfo> newroomsInfos = 
                new Tuple<Vector3Int, RoomInfo>(pos,new RoomInfo(roomsInfos.Count, Conncection, type, element));

             roomsInfos.Add(newroomsInfos);
        }

        public int[] GetPossibleConnects(Vector3Int[] allArounds)
        {
            int[] roomIdsConnect = { -1, -1, -1, -1, -1, -1 };
            
                for (int i = 0; i < allArounds.Length; i++)
                {
                    foreach (Tuple<Vector3Int, RoomInfo> roomInfo in roomsInfos)
                    {
                        if (roomInfo.Item1 == allArounds[i])
                        {
                            roomIdsConnect[i] = roomInfo.Item2.RoomID;
                            break;
                        }
                    }
                }
            
            return roomIdsConnect;
        }

        public List<RoomInfo> GetRoomList()
        {
            List<RoomInfo> RoomsInfos = new List<RoomInfo>();
            foreach (Tuple<Vector3Int, RoomInfo> roomInfo in roomsInfos)
            {
                print("---ExPos:"+ roomInfo.Item1.ToString());
                RoomsInfos.Add(roomInfo.Item2);
            }
            return RoomsInfos;
        }
        public List<Tuple<Vector3Int, RoomInfo>> GetFullRoomList()
        {
            return roomsInfos;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                foreach(Tuple<Vector3Int, RoomInfo> roomInfo in roomsInfos)
                {
                    foreach (RoomConnection roomConnection in roomInfo.Item2.Conncection)
                    {
                        print("Vector:"+ roomInfo.Item1 + "Room:" + roomInfo.Item2.RoomID + "RoomConnection id:" + roomConnection.targetRoomId + " Dir: " + roomConnection.connectionDir);
                    }
                    print("------");

                }
            }
        }
    }
}
