using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace dungeonduell
{
    public class SimpleCamHandler : MonoBehaviour
    {
        [SerializeField] private List<CinemachineVirtualCamera> cams;

        [Header("Overlay Animatoren")]
        [SerializeField] private Animator coverCam;       // Schwarzes Overlay
        [SerializeField] private Animator coverMapTop;    // Graues Minimap-Overlay

        [Header("Minimap Fokus")]
        [SerializeField] private MinimapCamManager minimapCamManager;
        [SerializeField] private RoomData roomData;


        private void Start()
        {
            minimapCamManager = FindObjectOfType<MinimapCamManager>();
            if (minimapCamManager == null)
            {
                Debug.LogWarning("MinimapCamManager not found in scene!");
                return;
            }

            // Player 1 Spawn suchen und als Startziel setzen
            GameObject p1Spawn = GameObject.FindWithTag("SpawnpointPlayer1");
            if (p1Spawn != null)
                minimapCamManager.SetFollowTarget(p1Spawn.transform, true);

            // Player 2 Spawn suchen und als Startziel setzen
            GameObject p2Spawn = GameObject.FindWithTag("SpawnpointPlayer2");
            if (p2Spawn != null)
                minimapCamManager.SetFollowTarget(p2Spawn.transform, false);
        }

        public void EnteringRoom(Collider2D collision)
        {
            for (int i = 0; i < cams.Count; i++)
            {
                if (collision.CompareTag("Player" + (i + 1)))
                {
                    cams[i].gameObject.SetActive(true);
                    cams[i].Follow = collision.transform;

                    coverCam.SetBool("InRoom", true);
                    coverMapTop.SetBool("InRoom_Map", true);

                    if (roomData != null && roomData.roomCenter != null)
                    {
                        minimapCamManager.SetFollowTarget(roomData.roomCenter, i == 0);
                        Debug.Log($"Minimap-Fokus gesetzt auf Raumzentrum für Player{i + 1}");
                    }
                    else
                    {
                        Debug.LogWarning($"[SimpleCamHandler] RoomData oder roomCenter fehlt im Inspector.");
                    }
                }
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
                coverCam.SetBool("InRoom", false);
                coverMapTop.SetBool("InRoom_Map", false);
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
