using UnityEngine;

namespace dungeonduell
{
    public class GymCorpseDespawn : MonoBehaviour
    {
        public float lifetime = 5f;
        public void CorpseDespawn()
        {
            Destroy(gameObject, lifetime);
        }
    }
}
