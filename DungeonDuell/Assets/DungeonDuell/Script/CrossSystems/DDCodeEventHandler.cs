using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
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
        public static event Action<int, int> LevelUpAvailable;
        public static void Trigger_LevelUpAvailable(int playerId, int upgradableCount) { LevelUpAvailable.Invoke(playerId, upgradableCount); }
        public static event Action<LevelUpOptions, String ,int> PlayerUpgrade;
        public static void Trigger_PlayerUpgrade(LevelUpOptions option, String playerReference, int amount) { PlayerUpgrade.Invoke(option, playerReference, amount); }
        public static event Action<List<PlayerData>> PlayerDataExposed;
        public static void Trigger_PlayerDataExposed(List<PlayerData> playerdatas) { PlayerDataExposed.Invoke(playerdatas); }


    }
}
