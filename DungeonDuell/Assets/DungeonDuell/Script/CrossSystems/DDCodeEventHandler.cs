using System;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    // When Refractoring the Code add the new Events here
    public class DdCodeEventHandler : MonoBehaviour
    {
        // Card Phase
        public static event Action DungeonConnected;

        public static void Trigger_DungeonConnected()
        {
            if (DungeonConnected != null) DungeonConnected.Invoke();
        }

        public static event Action NextPlayerTurn;

        public static void Trigger_NextPlayerTurn()
        {
            if (NextPlayerTurn != null) NextPlayerTurn.Invoke();
        }

        public static event Action<DisplayCard>
            CardSelected; // this might not be ideal but seems best for visual update and destroying later  

        public static void Trigger_CardSelected(DisplayCard card)
        {
            if (CardSelected != null) CardSelected.Invoke(card);
        }

        public static event Action<Card, bool> CardPlayed;

        public static void Trigger_CardPlayed(Card card, bool player1Played)
        {
            if (CardPlayed != null) CardPlayed.Invoke(card, player1Played);
        }

        public static event Action<Card, bool> CardToBeShelled;

        public static void Trigger_CardToShelled(Card oldcard, bool player1Played)
        {
            if (CardToBeShelled != null) CardToBeShelled.Invoke(oldcard, player1Played);
        }

        public static event Action<bool> PlayedAllCards;

        public static void Trigger_PlayedAllCards(bool player1)
        {
            if (PlayedAllCards != null) PlayedAllCards.Invoke(player1);
        }

        public static event Action<Card, Vector3Int> PreSetCardSetOnTilemap;

        public static void Trigger_PreSetCardSetOnTilemap(Card card, Vector3Int point)
        {
            if (PreSetCardSetOnTilemap != null) PreSetCardSetOnTilemap.Invoke(card, point);
        }

        public static event Action<bool[]> CardRotating;

        public static void Trigger_CardRotating(bool[] newRot)
        {
            if (CardRotating != null) CardRotating.Invoke(newRot);
        }

        public static event Action FinalRoundInDungeon;

        public static void Trigger_FinalRoundInDungeon()
        {
            if (FinalRoundInDungeon != null) FinalRoundInDungeon.Invoke();
        }

        public static event Action<int, int> LevelUpAvailable;

        public static void Trigger_LevelUpAvailable(int playerId, int upgradableCount)
        {
            if (LevelUpAvailable != null) LevelUpAvailable.Invoke(playerId, upgradableCount);
        }

        public static event Action<LevelUpOptions, string, int> PlayerUpgrade;

        public static void Trigger_PlayerUpgrade(LevelUpOptions option, string playerReference, int amount)
        {
            if (PlayerUpgrade != null) PlayerUpgrade.Invoke(option, playerReference, amount);
        }

        public static event Action<List<PlayerData>, int> PlayerDataExposed;

        public static void Trigger_PlayerDataExposed(List<PlayerData> playerdatas, int currentRound)
        {
            if (PlayerDataExposed != null) PlayerDataExposed.Invoke(playerdatas, currentRound);
        }

        public static event Action BridgeMode;

        public static void Trigger_BridgeMode()
        {
            if (BridgeMode != null) BridgeMode.Invoke();
        }
    }
}