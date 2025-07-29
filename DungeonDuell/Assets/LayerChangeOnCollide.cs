using Unity.VisualScripting;
using UnityEngine;

namespace dungeonduell
{
    public class LayerChangeOnCollide : MonoBehaviour
    {
        public int layerOnCollide = 100;
        public int layerOffCollide = 0;
        private SpriteRenderer _sprite;

        void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (_sprite != null) _sprite.sortingOrder = layerOnCollide;
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (_sprite != null) _sprite.sortingOrder = layerOffCollide;
        }
    }
}