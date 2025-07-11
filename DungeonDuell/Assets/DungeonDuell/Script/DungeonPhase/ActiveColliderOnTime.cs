using System.Collections;
using UnityEngine;

namespace dungeonduell
{
    public class ActiveColliderOnTime : MonoBehaviour
    {
        [SerializeField] private float time = 3;
        private Collider2D _gameObjectCollider;
        public bool on = true;

        private void Awake()
        {
            if (on)
            {
                _gameObjectCollider = GetComponent<Collider2D>();
                _gameObjectCollider.enabled = false;
                StartCoroutine(StartAfterSecounds());
            }
           
        }

        private IEnumerator StartAfterSecounds()
        {
            yield return new WaitForSeconds(time);
            _gameObjectCollider.enabled = true;
        }

        public void Interrupt()
        {
            on = false;
            _gameObjectCollider.enabled = true;
        }
    }
}