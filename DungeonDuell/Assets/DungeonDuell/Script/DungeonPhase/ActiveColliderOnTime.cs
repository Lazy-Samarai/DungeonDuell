using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace dungeonduell
{
    public class ActiveColliderOnTime : MonoBehaviour
    {
        [SerializeField] float time = 3;
        Collider2D _gameObjectCollider;

        void Awake()
        {
            _gameObjectCollider = GetComponent<Collider2D>();
            _gameObjectCollider.enabled = false;
            StartCoroutine(StartAfterSecounds());
        }

        IEnumerator StartAfterSecounds()
        {
            yield return new WaitForSeconds(time);
            _gameObjectCollider.enabled = true;
        }
    }
}
