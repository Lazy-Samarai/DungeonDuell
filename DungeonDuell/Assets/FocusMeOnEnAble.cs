using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace dungeonduell
{
    public class FocusMeOnEnAble : MonoBehaviour
    {
        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }
    }
}
