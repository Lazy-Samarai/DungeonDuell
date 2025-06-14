using System.Collections.Generic;
using UnityEngine;

namespace dungeonduell
{
    public class SetPortraits : MonoBehaviour, IObserver
    {
        [SerializeField] Sprite[] portraits;
        [SerializeField] bool player1;

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToAllEvents();
        }

        public void SubscribeToEvents()
        {
            DdCodeEventHandler.PlayerDataExposed += SetMask;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.PlayerDataExposed -= SetMask;
        }

        private void SetMask(List<PlayerData> playerDatas, int currentRound)
        {
            UpgradeMask currentMask = (UpgradeMask)playerDatas[player1 ? 0 : 1].inventory.Content[0];
            if (currentMask == null)
            {
                GetComponent<UnityEngine.UI.Image>().sprite = portraits[0];
            }
            else
            {
                GetComponent<UnityEngine.UI.Image>().sprite = portraits[currentMask.skinId - 1];
            }
        }
    }
}