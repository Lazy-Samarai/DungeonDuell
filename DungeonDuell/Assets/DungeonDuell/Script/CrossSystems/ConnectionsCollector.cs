using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dungeonduell
{
    public class ConnectionsCollector : MonoBehaviour
    {
        [SerializeField] private List<RoomType> filteredRoomTypeFromFirstCoin;
        public List<Tuple<Vector3Int, RoomInfo>> RoomsInfos = new();

        private void Awake()
        {
            var objs = FindObjectsByType<ConnectionsCollector>(FindObjectsSortMode.None);

            if (objs.Length > 1) Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            //Debug.Log("OnEnable called");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }


        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // called second
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // So the map is not seen in Dunegeon Phase
            if (scene.buildIndex == 2)
                transform.GetChild(0).gameObject.SetActive(false);
            else
                transform.GetChild(0).gameObject.SetActive(true);
        }

        public void AddRoom(Vector3Int pos, List<RoomConnection> conncection, RoomType type, RoomElement element,
            List<ConnectionDir> newAllowedDoors, int owner, int territoryOwner)
        {
            var newroomsInfos =
                new Tuple<Vector3Int, RoomInfo>(pos,
                    new RoomInfo(RoomsInfos.Count, conncection, type, element, newAllowedDoors, owner,
                        !filteredRoomTypeFromFirstCoin.Contains(type), territoryOwner));

            RoomsInfos.Add(newroomsInfos);
        }

        public int[]
            GetPossibleConnects(Vector3Int[] allArounds, bool[] allowedDoors,
                bool forceOnRoom) // TODO Allround probaly not parll
        {
            // FocredRoom for scenario that player connecting tile is placed but connection is not two sided even after adding one from set tile to target

            int[] roomIdsConnect = { -1, -1, -1, -1, -1, -1 };

            for (var i = 0; i < allArounds.Length; i++)
                if (allowedDoors[i]) // Dont Tracking for Connection are not allowed 
                    foreach (var roomInfo in RoomsInfos)
                        if ((roomInfo.Item1 == allArounds[i]) & (forceOnRoom |
                                                                 roomInfo.Item2.AllowedDoors.Contains(((ConnectionDir)i)
                                                                     .GetInvert())))
                        {
                            roomIdsConnect[i] = roomInfo.Item2.RoomID;
                            break;
                        }

            return roomIdsConnect;
        }

        public List<RoomInfo> GetRoomList()
        {
            var roomsInfos = new List<RoomInfo>();
            foreach (var roomInfo in this.RoomsInfos) roomsInfos.Add(roomInfo.Item2);

            return roomsInfos;
        }

        public List<Tuple<Vector3Int, RoomInfo>> GetFullRoomList()
        {
            return RoomsInfos;
        }
    }
}