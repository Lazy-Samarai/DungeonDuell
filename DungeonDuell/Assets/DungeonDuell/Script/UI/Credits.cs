using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class Credits : MonoBehaviour
    {
        public RectTransform floatingText;
        public float speed;

        // L�sst den Text automatsich ablaufen
        void Update()
        {
            floatingText.anchoredPosition += new Vector2(0f, speed);
        }
    }
}