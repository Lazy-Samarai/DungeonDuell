using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class BarrierHealth : MonoBehaviour
    {
        [SerializeField] int hp = 2;
        // Start is called before the first frame update

        void OnTriggerEnter2D(Collider2D collision)
        {
            hp--;
            if (hp <= 0)
            {
                Destroy(gameObject);
            }

        }
    }
}
