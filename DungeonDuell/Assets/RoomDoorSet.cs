using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace dungeonduell
{
    [CreateAssetMenu]
    public class RoomDoorSet : ScriptableObject
    {
        [System.Serializable]
        public struct DirBar { public bool TopLeft; public bool TopRight; public bool Left; public bool Right; public bool BottonRight; public bool BottonLeft; }
        public DirBar m_bar;

        public void Shift()
        {

        }

    }
}
