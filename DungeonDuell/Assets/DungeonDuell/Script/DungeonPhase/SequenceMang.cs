using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace dungeonduell
{
    public class SequenceMang : MonoBehaviour, IObserver
    {
        const float BaseTime = 30;
        const float TimeMorePerRound = 20;
        
        [SerializeField] float timeRound = 150.0f;
        
        [SerializeField] bool timeRunning = false;
        [SerializeField] bool finalRound = false;
        SceneLoading sceneLoading;
        [SerializeField] TextMeshProUGUI timerText;
        // Start is called before the first frame update
        void Start()
        {
            sceneLoading = GetComponentInChildren<SceneLoading>();
        }

        // Update is called once per frame
        void Update()
        {
            if (timeRunning & !finalRound)
            {
                timeRound -= Time.deltaTime;
                if (timeRound < 0)
                {
                    BackToCardPhase();
                }
                int totalSeconds = (int)Mathf.Floor(timeRound);
                // int minutes = totalSeconds / 60;
                // int seconds = totalSeconds % 60;


                timerText.text = totalSeconds.ToString();
            }
           


        }
        public void BackToCardPhase()
        {
            sceneLoading.ToTheHex();
        }
        public void Reseting()
        {
            
            ConnectionsCollector connectionsCollector = FindObjectOfType<ConnectionsCollector>();
            PlayerDataManager playerMang = FindObjectOfType<PlayerDataManager>();
            Destroy(playerMang.gameObject);
            Destroy(connectionsCollector.gameObject);
            BackToCardPhase();
        }
        public void DisableTimer()
        {
            finalRound = true;
            timeRunning = false;
            timerText.text = "X";

        }

        private void SetTimer(List<PlayerData> d, int currentRound)
        {
            timeRound = BaseTime + (currentRound * TimeMorePerRound);
        }
        void OnEnable()
        {
            SubscribeToEvents();
        }
        void OnDisable()
        {
            UnsubscribeToAllEvents();
        }
        public void SubscribeToEvents()
        {
            DDCodeEventHandler.FinalRoundInDungeon += DisableTimer;
            DDCodeEventHandler.PlayerDataExposed += SetTimer;
        }

        public void UnsubscribeToAllEvents()
        {
            DDCodeEventHandler.FinalRoundInDungeon -= DisableTimer;
            DDCodeEventHandler.PlayerDataExposed += SetTimer;
        }


    }
}
