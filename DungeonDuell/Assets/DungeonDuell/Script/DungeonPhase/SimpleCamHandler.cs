using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace dungeonduell
{
    public class SimpleCamHandler : MonoBehaviour
    {
        [SerializeField] private List<CinemachineVirtualCamera> cams;
        [SerializeField] private Animator coverCam;       // Schwarzes Overlay
        [SerializeField] private Animator coverMapTop;    // Graues Minimap-Overlay

        public void EnteringRoom(Collider2D collision)
        {
            for (var index = 0; index < cams.Count; index++)
            {
                if (collision.tag == "Player" + (index + 1))
                {
                    cams[index].gameObject.SetActive(true);
                    cams[index].Follow = collision.transform;

                    if (!coverCam.GetBool("InRoom"))
                        coverCam.SetBool("InRoom", true);

                    if (!coverMapTop.GetBool("InRoom"))
                        coverMapTop.SetBool("InRoom", true);
                }
            }
        }

        public void ExitingRoom(Collider2D collision)
        {
            for (var index = 0; index < cams.Count; index++)
            {
                if (collision.tag == "Player" + (index + 1))
                {
                    cams[index].gameObject.SetActive(false);

                    if (AllCamsOff())
                    {
                        coverCam.SetBool("InRoom", false);
                        coverMapTop.SetBool("InRoom", false);
                    }
                }
            }
        }

        private bool AllCamsOff()
        {
            foreach (var cam in cams)
                if (cam.gameObject.activeSelf)
                    return false;

            return true;
        }
    }
}