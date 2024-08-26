using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace dungeonduell
{
    public class SimpleCamHandler : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera cam;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == "Player")
            {
                cam.gameObject.SetActive(true);
            }
           
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                cam.gameObject.SetActive(false);
            }
        }

    }
}
