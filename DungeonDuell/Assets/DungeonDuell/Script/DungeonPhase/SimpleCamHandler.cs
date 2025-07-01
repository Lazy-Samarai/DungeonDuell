using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using System.Linq;

namespace dungeonduell
{
    public class SimpleCamHandler : MonoBehaviour
    {
        [SerializeField] private List<CinemachineVirtualCamera> cams;

        [Header("Overlay Animatoren")] 
        [SerializeField] private Animator coverCam; // Schwarzes Overlay
        [SerializeField] private Animator coverMapTop; // Graues Minimap-Overlay

        [Header("Minimap Fokus")] 
        [SerializeField] private MinimapCamManager minimapCamManager;
        [SerializeField] private RoomData roomData;

        private const string AnimatorInRoom = "InRoom";
        private const string AnimatorInRoomMap = "InRoom_Map";

        private void Start()
        {
            minimapCamManager = FindFirstObjectByType<MinimapCamManager>();
            if (minimapCamManager == null)
            {
                Debug.LogWarning("MinimapCamManager not found in scene!");
                return;
            }

            if (minimapCamManager.systemActive)
            {
                var allObjects = FindObjectsOfType<GameObject>();

                var p1Spawn = allObjects.FirstOrDefault(obj => obj.CompareTag("SpawnpointPlayer1"));
                if (p1Spawn != null)
                    minimapCamManager.SetFollowTarget(p1Spawn.transform, true);

                var p2Spawn = allObjects.FirstOrDefault(obj => obj.CompareTag("SpawnpointPlayer2"));
                if (p2Spawn != null)
                    minimapCamManager.SetFollowTarget(p2Spawn.transform, false);
            }
            else
            {
                coverMapTop.gameObject.SetActive(false);
            }
        }

        public void EnteringRoom(Collider2D collision)
        {
            for (int i = 0; i < cams.Count; i++)
            {
                if (collision.CompareTag("Player" + (i + 1)))
                {
                    cams[i].gameObject.SetActive(true);
                    cams[i].Follow = collision.transform;

                    coverCam.SetBool(AnimatorInRoom, true);
                    if (minimapCamManager.systemActive) coverMapTop.SetBool(AnimatorInRoomMap, true);

                    StartCoroutine(DelayedFocusSet(i == 0, 0.25f));

                    if (roomData != null && minimapCamManager.systemActive)
                        roomData.SetMapWallsActive(true); // MapWalls AN im aktiven Raum
                }
            }
        }

        private IEnumerator DelayedFocusSet(bool isPlayer1, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (roomData != null && roomData.roomCenter != null)
            {
                minimapCamManager.SetFollowTarget(roomData.roomCenter, isPlayer1);
            }
            else
            {
                Debug.LogWarning("[SimpleCamHandler] roomData oder roomCenter fehlt.");
            }
        }

        public void ExitingRoom(Collider2D collision)
        {
            for (int i = 0; i < cams.Count; i++)
            {
                if (collision.CompareTag("Player" + (i + 1)))
                {
                    cams[i].gameObject.SetActive(false);
                }
            }

            if (AllCamsOff())
            {
                coverCam.SetBool(AnimatorInRoom, false);
                if (minimapCamManager.systemActive) coverMapTop.SetBool(AnimatorInRoomMap, false);

                if (roomData != null)
                    roomData.SetMapWallsActive(false); // MapWalls AUS wenn niemand mehr im Raum
            }
        }

        private bool AllCamsOff()
        {
            foreach (var cam in cams)
            {
                if (cam.gameObject.activeSelf)
                    return false;
            }

            return true;
        }
    }
}
