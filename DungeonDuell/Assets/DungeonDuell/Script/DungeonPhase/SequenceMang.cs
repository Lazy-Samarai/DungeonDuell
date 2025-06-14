using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace dungeonduell
{
    public class SequenceMang : MonoBehaviour, IObserver
    {
        private const float BaseTime = 30;
        private const float TimeMorePerRound = 20;

        [SerializeField] private float timeRound = 150.0f;

        [SerializeField] private bool timeRunning;
        [SerializeField] private bool finalRound;

        [SerializeField] private TextMeshProUGUI timerText;
        private SceneLoading _sceneLoading;

        [SerializeField] private float allRoomVisitedTime = 10;

        // Start is called before the first frame update
        private void Start()
        {
            _sceneLoading = GetComponentInChildren<SceneLoading>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (timeRunning & !finalRound)
            {
                timeRound -= Time.deltaTime;
                if (timeRound < 0) BackToCardPhase();

                var totalSeconds = (int)Mathf.Floor(timeRound);
                // int minutes = totalSeconds / 60;
                // int seconds = totalSeconds % 60;


                timerText.text = totalSeconds.ToString();
            }
        }

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
            DdCodeEventHandler.FinalRoundInDungeon += DisableTimer;
            DdCodeEventHandler.PlayerDataExposed += SetTimer;
            DdCodeEventHandler.AllRoomVisited += OnAllRoomVisited;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.FinalRoundInDungeon -= DisableTimer;
            DdCodeEventHandler.PlayerDataExposed -= SetTimer;
            DdCodeEventHandler.AllRoomVisited -= OnAllRoomVisited;
        }

        public void BackToCardPhase()
        {
            DdCodeEventHandler.Trigger_SceneTransition();
            //_sceneLoading.ToTheHex();
        }

        public void DisableTimer()
        {
            finalRound = true;
            timeRunning = false;
            timerText.text = "X";
        }

        private void SetTimer(List<PlayerData> d, int currentRound)
        {
            timeRound = BaseTime + currentRound * TimeMorePerRound;
        }

        private void OnAllRoomVisited()
        {
            if (timeRound > allRoomVisitedTime && !finalRound)
            {
                timeRound = allRoomVisitedTime;
            }
        }
    }
}