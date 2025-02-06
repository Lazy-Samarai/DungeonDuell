using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    // When Refractoring the Code add the new Events here
    public class DDCodeEventHandler : MonoBehaviour
    {

        public static event Action DungeonConnected;
        public static void Trigger_DungeonConnected() { DungeonConnected.Invoke(); }

    }
}
