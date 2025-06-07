using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace dungeonduell
{
    public class SimpleCamHandler : MonoBehaviour
    {
        [SerializeField] private List<CinemachineVirtualCamera> cams;
        [SerializeField] private Animator coverCam;

        public void EnteringRoom(Collider2D collision)
        {
            for (var index = 0; index < cams.Count; index++)
                if (collision.tag == "Player" + (index + 1))
                {
                    cams[index].gameObject.SetActive(true);
                    if (!coverCam.GetBool("InRoom")) coverCam.SetBool("InRoom", true);
                    cams[index].Follow = collision.transform;
                }
        }

        public void ExitingRoom(Collider2D collision)
        {
            for (var index = 0; index < cams.Count; index++)
                if (collision.tag == "Player" + (index + 1))
                {
                    cams[index].gameObject.SetActive(false);
                    if (AllCamsOff()) coverCam.SetBool("InRoom", false);
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