using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace dungeonduell
{
    public class SimpleCamHandler : MonoBehaviour
    {
        [SerializeField] List<CinemachineVirtualCamera> cams;
        [SerializeField] Animator coverCam;
    
        private void OnTriggerEnter2D(Collider2D collision)
        {
            for (int index = 0; index < cams.Count; index++)
            {
                if (collision.tag == "Player" + (index +1))
                {
                    cams[index].gameObject.SetActive(true);
                    if (!coverCam.GetBool("InRoom"))
                    {
                        coverCam.SetBool("InRoom", true);
                    }              
                    cams[index].Follow = collision.transform;
                }
            }    
           
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            for (int index = 0; index < cams.Count; index++)
            {
                if (collision.tag == "Player" + (index + 1))
                {
                    cams[index].gameObject.SetActive(false);
                    if (AllCamsOff())
                    {
                        coverCam.SetBool("InRoom", false);
                    }
                   
                }
            }
       
        }
        private bool AllCamsOff()
        {
            foreach(CinemachineVirtualCamera cam in cams) 
            {
                if (cam.gameObject.activeSelf)
                {
                  
                    return false;
                }
            }
            return true;
        }
       

    }
}
