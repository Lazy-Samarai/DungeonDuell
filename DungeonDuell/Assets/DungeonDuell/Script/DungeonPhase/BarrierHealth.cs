using UnityEngine;

namespace dungeonduell
{
    public class BarrierHealth : MonoBehaviour
    {
        [SerializeField] private int hp = 2;
        // Start is called before the first frame update

        private void OnTriggerEnter2D(Collider2D collision)
        {
            hp--;
            if (hp <= 0) Destroy(gameObject);
        }
    }
}