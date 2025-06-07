using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace dungeonduell
{
    public class MetaHpBar : MonoBehaviour, IObserver
    {
        [SerializeField] private bool player1;

        [SerializeField] private bool showIfHealthReduction; // for in Dungeon Phase 

        [FormerlySerializedAs("NotUsedHealthBar")] [SerializeField]
        private Transform notUsedHealthBar;

        private MMProgressBar _mMProgressBar;

        private void OnEnable()
        {
            _mMProgressBar = GetComponent<MMProgressBar>();
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToAllEvents();
        }

        public void SubscribeToEvents()
        {
            DdCodeEventHandler.PlayerDataExposed += UpdateMetaHealthBar;
            DdCodeEventHandler.DungeonConnected += DeactivateNotUsedHealth;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.PlayerDataExposed -= UpdateMetaHealthBar;
            DdCodeEventHandler.DungeonConnected -= DeactivateNotUsedHealth;
        }

        private void UpdateMetaHealthBar(List<PlayerData> playerDatas, int round)
        {
            var playerData = playerDatas[player1 ? 0 : 1];

            _mMProgressBar.SetBar(showIfHealthReduction ? playerData.metaHp - playerData.maxHealth : playerData.metaHp,
                0, playerData.maxMetaHp);

            if (notUsedHealthBar != null)
                notUsedHealthBar.localScale = new Vector3(
                    (playerData.metaHp - playerData.maxHealth) / playerData.maxMetaHp,
                    notUsedHealthBar.localScale.y, notUsedHealthBar.localScale.z);
        }

        public void DeactivateNotUsedHealth()
        {
            notUsedHealthBar.gameObject.SetActive(false);
        }
    }
}