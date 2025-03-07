using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class LockDown : MonoBehaviour
    {
        [SerializeField] List<GameObject> allBarriers;
       public void GoinginLockDown(bool lockingDown)
       {
            print("Lock");
            foreach(GameObject obj in allBarriers)
            {
                obj.SetActive(lockingDown);
            }
       }
    }
}
