using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace dungeonduell
{
    public class TSCircleRotation : MonoBehaviour
    {
        public float rotationSpeed = 100f;

        public bool unscaledTime = false;

        private void Start()
        {
            float duration = 360f / rotationSpeed;
            transform.DORotate(new Vector3(0, 0, 360f), duration, RotateMode.FastBeyond360)
                     .SetEase(Ease.Linear)
                     .SetLoops(-1, LoopType.Restart)
                     .SetUpdate(unscaledTime);
        }
    }
}
