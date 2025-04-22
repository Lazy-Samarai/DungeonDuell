using System;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    // When Refractoring the Code add the new Events here
    public class DdCodeEventHandler : MonoBehaviour
    {
        // Card Phase
        public static event Action DungeonConnected;

        public static void Trigger_DungeonConnected()
        {
            DungeonConnected.Invoke();
        }

        public static event Action NextPlayerTurn;

        public static void Trigger_NextPlayerTurn()
        {
            NextPlayerTurn.Invoke();
        }

        public static event Action<DisplayCard>
            CardSelected; // this might not be ideal but seems best for visual update and destroying later  

        public static void Trigger_CardSelected(DisplayCard card)
        {
            CardSelected.Invoke(card);
        }

        public static event Action<Card, bool> CardPlayed;

        public static void Trigger_CardPlayed(Card card, bool player1Played)
        {
            CardPlayed.Invoke(card, player1Played);
        }

        public static event Action<Card, bool> CardToBeShelled;

        public static void Trigger_CardToShelled(Card oldcard, bool player1Played)
        {
            CardToBeShelled.Invoke(oldcard, player1Played);
        }

        public static event Action<bool> PlayedAllCards;

        public static void Trigger_PlayedAllCards(bool player1)
        {
            PlayedAllCards.Invoke(player1);
        }

        public static event Action<Card, Vector3Int> PreSetCardSetOnTilemap;

        public static void Trigger_PreSetCardSetOnTilemap(Card card, Vector3Int point)
        {
            PreSetCardSetOnTilemap.Invoke(card, point);
        }

        public static event Action<bool[]> CardRotating;

        public static void Trigger_CardRotating(bool[] newRot)
        {
            CardRotating.Invoke(newRot);
        }

        public static event Action FinalRoundInDungeon;

        public static void Trigger_FinalRoundInDungeon()
        {
            FinalRoundInDungeon.Invoke();
        }

        public static event Action<int, int> LevelUpAvailable;

        public static void Trigger_LevelUpAvailable(int playerId, int upgradableCount)
        {
            LevelUpAvailable.Invoke(playerId, upgradableCount);
        }

        public static event Action<LevelUpOptions, string, int> PlayerUpgrade;

        public static void Trigger_PlayerUpgrade(LevelUpOptions option, string playerReference, int amount)
        {
            PlayerUpgrade.Invoke(option, playerReference, amount);
        }

        public static event Action<List<PlayerData>, int> PlayerDataExposed;

        public static void Trigger_PlayerDataExposed(List<PlayerData> playerdatas, int currentRound)
        {
            PlayerDataExposed.Invoke(playerdatas, currentRound);
        }
    }
}