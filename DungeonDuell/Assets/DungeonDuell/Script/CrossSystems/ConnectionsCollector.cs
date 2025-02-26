using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace dungeonduell
{
    public class ConnectionsCollector : MonoBehaviour
    {    
        public List<Tuple<Vector3Int, RoomInfo>> roomsInfos = new List<Tuple<Vector3Int, RoomInfo>>();

        void OnEnable()
        {
            //Debug.Log("OnEnable called");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void Awake()
        {
            ConnectionsCollector[] objs = FindObjectsOfType<ConnectionsCollector>();

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }
        // called second
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // So the map is not seen in Dunegeon Phase
           if(scene.buildIndex == 1)
           {
                transform.GetChild(0).gameObject.SetActive(false);
           }
           else
           {
                transform.GetChild(0).gameObject.SetActive(true);
           }
        }

        public void AddRoom(Vector3Int pos, List<RoomConnection> Conncection,RoomType type, RoomElement element, List<ConnectionDir> newAllowedDoors,int owner)
        {
            Tuple<Vector3Int, RoomInfo> newroomsInfos = 
                new Tuple<Vector3Int, RoomInfo>(pos,new RoomInfo(roomsInfos.Count, Conncection, type, element, newAllowedDoors, owner));

             roomsInfos.Add(newroomsInfos);
        }

        public int[] GetPossibleConnects(Vector3Int[] allArounds,bool[] allowedDoors,bool forceOnRoom) // TODO Allround probaly not parll
        {
            // FocredRoom for scenario that player connecting tile is placed but connection is not two sided even after adding one from set tile to target

            int[] roomIdsConnect = { -1, -1, -1, -1, -1, -1 };

            for (int i = 0; i < allArounds.Length; i++)
                {
                    if (allowedDoors[i]) // Dont Tracking for Connection are not allowed 
                    {                       
                        foreach (Tuple<Vector3Int, RoomInfo> roomInfo in roomsInfos)
                        {
                            if (roomInfo.Item1 == allArounds[i] & (forceOnRoom | (roomInfo.Item2.allowedDoors.Contains(((ConnectionDir)i).GetInvert()))))
                            {
                                roomIdsConnect[i] = roomInfo.Item2.RoomID;
                                break;
                            }
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
        void OnDisable()
        {
            //Debug.Log("OnDisable");
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
