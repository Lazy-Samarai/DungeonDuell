using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    // When Refractoring the Code add the new Events here
    public class DDCodeEventHandler : MonoBehaviour
    {
        // Card Phase
        public static event Action DungeonConnected;
        public static void Trigger_DungeonConnected() { DungeonConnected.Invoke(); }
        public static event Action NextPlayerTurn;
        public static void Trigger_NextPlayerTurn() { NextPlayerTurn.Invoke(); }
        public static event Action<DisplayCard> CardSelected; // this might not be ideal but seems best for visual update and destroying later  
        public static void Trigger_CardSelected(DisplayCard card) { CardSelected.Invoke(card); }
        public static event Action<Card, bool> CardPlayed;
        public static void Trigger_CardPlayed(Card card, bool Player1Played) { CardPlayed.Invoke(card, Player1Played); }
        public static event Action FinalRoundInDungeon;
        public static void Trigger_FinalRoundInDungeon() { FinalRoundInDungeon.Invoke(); }

    }
}
