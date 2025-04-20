using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace dungeonduell
{
    public class MetaHpBar : MonoBehaviour, IObserver
    {
        [SerializeField] private bool player1;

        [SerializeField] private bool showIfHealthReduction; // for in Dungeon Phase 

        [SerializeField] private Transform NotUsedHealthBar;
        private MMProgressBar mMProgressBar;

        private void OnEnable()
        {
            mMProgressBar = GetComponent<MMProgressBar>();
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToAllEvents();
        }

        public void SubscribeToEvents()
        {
            DDCodeEventHandler.PlayerDataExposed += UpdateMetaHealthBar;
            DDCodeEventHandler.DungeonConnected += DeactivateNotUsedHealth;
        }

        public void UnsubscribeToAllEvents()
        {
            DDCodeEventHandler.PlayerDataExposed -= UpdateMetaHealthBar;
            DDCodeEventHandler.DungeonConnected -= DeactivateNotUsedHealth;
        }

        private void UpdateMetaHealthBar(List<PlayerData> playerDatas, int round)
        {
            var playerData = playerDatas[player1 ? 0 : 1];

            mMProgressBar.SetBar(showIfHealthReduction ? playerData.MetaHp - playerData.MaxHealth : playerData.MetaHp,
                0, playerData.MaxMetaHp);

            if (NotUsedHealthBar != null)
                NotUsedHealthBar.localScale = new Vector3(
                    (playerData.MetaHp - playerData.MaxHealth) / playerData.MaxMetaHp,
                    NotUsedHealthBar.localScale.y, NotUsedHealthBar.localScale.z);
        }

        public void DeactivateNotUsedHealth()
        {
            NotUsedHealthBar.gameObject.SetActive(false);
        }
    }
}