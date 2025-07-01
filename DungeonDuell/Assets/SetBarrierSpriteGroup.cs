using UnityEngine;

namespace dungeonduell
{
    public class SetBarrierSpriteGroup : MonoBehaviour
    {
        SetBarrierSprite[] _setBarrierSprites;

        void Start()
        {
            _setBarrierSprites = GetComponentsInChildren<SetBarrierSprite>();
        }

        public void BarrierVisualDamage()
        {
            foreach (SetBarrierSprite setBarrierSprite in _setBarrierSprites)
            {
                setBarrierSprite.SetDamageSprite();
            }
        }

        public void BarrierVisualDeath()
        {
            foreach (SetBarrierSprite setBarrierSprite in _setBarrierSprites)
            {
                setBarrierSprite.SetDeathSprite();
            }
        }
    }
}