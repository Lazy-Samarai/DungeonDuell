using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;


namespace dungeonduell
{
    public class UiSelectSfx : MonoBehaviour, ISelectHandler, IPointerEnterHandler
    {
        // globale Sperre für Select-Sounds
        private static float s_suppressSelectUntil = -1f;

        [SerializeField] private float initialSuppressSeconds = 0.5f; // beim Szenenstart stumm
        [SerializeField] private StudioEventEmitter selectEmitter;     // Emitter: Trigger= None

        private void Awake()
        {
            // nur einmal pro Szene initialisieren
            if (s_suppressSelectUntil < 0f)
                s_suppressSelectUntil = Time.unscaledTime + initialSuppressSeconds;
        }

        public static void SuppressSelectFor(float seconds)
        {
            float t = Time.unscaledTime + Mathf.Max(0f, seconds);
            if (t > s_suppressSelectUntil) s_suppressSelectUntil = t;
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (Time.unscaledTime < s_suppressSelectUntil) return;

        if (selectEmitter != null) selectEmitter.Play();

        }

        // Wenn du Hover-Sound auch unterdrücken willst, hier gleiches Gate:
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Time.unscaledTime < s_suppressSelectUntil) return;

            
            if (selectEmitter != null) selectEmitter.Play();
        }
    }
}
