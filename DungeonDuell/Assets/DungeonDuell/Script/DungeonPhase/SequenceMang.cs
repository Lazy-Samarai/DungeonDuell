using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

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

        [SerializeField] private EventReference countdownSfx;
        private bool countdownSfxPlayed = false;

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
                if (timeRound <= 1) BackToCardPhase();

                int displaySeconds = Mathf.Max(0, Mathf.FloorToInt(timeRound));
                timerText.text = displaySeconds.ToString();

                if (!countdownSfxPlayed && timeRound <= 4f)
                {
                    countdownSfxPlayed = true;
                    RuntimeManager.PlayOneShot(countdownSfx, transform.position);
                }
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
            DdCodeEventHandler.Trigger_AtmosphereLevelChanged(AtmoLevel.Atmo_Action);
        }

        private void SetTimer(List<PlayerData> d, int currentRound)
        {
            timeRound = BaseTime + currentRound * TimeMorePerRound;
            countdownSfxPlayed = false;
        }

        private void OnAllRoomVisited()
        {
            if (timeRound > allRoomVisitedTime && !finalRound)
            {
                timeRound = allRoomVisitedTime;
                countdownSfxPlayed = false;
            }
        }
    }
}