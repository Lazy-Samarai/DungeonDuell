using UnityEngine;

namespace dungeonduell
{
    public class RoomBarrier : MonoBehaviour
    {
        [SerializeField] GameObject[] mainBarrierObject;
        bool _barrierReady = true;

        public void LockdownBarrier(bool lockdown)
        {
            if (_barrierReady)
            {
                if (lockdown)
                {
                    foreach (Transform child in transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                }
                else
                {
                    foreach (GameObject child in mainBarrierObject)
                    {
                        child.gameObject.SetActive(false);
                    }

                    GetComponentInChildren<SetBarrierSpriteGroup>().BarrierVisualDeath();
                }
            }
        }

        public void SetBarrierReady(bool on)
        {
            _barrierReady = on;
        }
    }
}