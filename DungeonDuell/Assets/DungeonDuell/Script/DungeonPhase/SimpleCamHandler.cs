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

        private void Start()
        {
            // Suche zur Laufzeit das globale MinimapCamManager-Objekt
            minimapCamManager = FindObjectOfType<MinimapCamManager>();
            if (minimapCamManager == null)
            {
                Debug.LogWarning("MinimapCamManager not found in scene!");
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

                    coverCam.SetBool("InRoom", true);
                    coverMapTop.SetBool("InRoom", true);

                    // Minimap auf Raumzentrum setzen
                    Transform roomCenter = collision.transform.GetComponentInParent<RoomData>()?.roomCenter;
                    if (roomCenter != null)
                    {
                        minimapCamManager.SetFollowTarget(roomCenter, i == 0);
                    }
                    else
                    {
                        Debug.LogWarning("RoomCenter missing on player parent.");
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
                coverMapTop.SetBool("InRoom", false);
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
