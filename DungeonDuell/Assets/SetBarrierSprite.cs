using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace dungeonduell
{
    public class SetBarrierSprite : MonoBehaviour
    {
        [SerializeField] private Sprite damageSprite;
        [SerializeField] private Sprite deathSprite;
        SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void SetSprite(Sprite newSprite)
        {
            _spriteRenderer.sprite = newSprite;
        }

        public void SetDamageSprite()
        {
            SetSprite(damageSprite);
        }

        public void SetDeathSprite()
        {
            if (deathSprite == null)
            {
                transform.gameObject.SetActive(false);
                return;
            }

            SetSprite(deathSprite);
        }
    }
}