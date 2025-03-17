using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public interface IObserver
    {
        public void SubscribeToEvents();
        public void UnsubscribeToAllEvents(); 
    }
}
