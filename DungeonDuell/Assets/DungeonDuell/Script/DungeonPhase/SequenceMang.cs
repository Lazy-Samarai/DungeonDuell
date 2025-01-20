using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace dungeonduell
{
    public class SequenceMang : MonoBehaviour
    {
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
            if(timeRunning & !finalRound)
            {
                timeRound -= Time.deltaTime;
                if(timeRound < 0)
                {
                    BackToCardPhase();
                }
                int totalSeconds = (int)Mathf.Floor(timeRound);
                // int minutes = totalSeconds / 60;
                // int seconds = totalSeconds % 60;


                timerText.text = totalSeconds.ToString();
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                BackToCardPhase();
            }

          
        }
        public void BackToCardPhase()
        {
            sceneLoading.ToTheHex();
        }
        public void Reseting()
        {
            ConnectionsCollector connectionsCollector = FindObjectOfType<ConnectionsCollector>();
            LivesManager livesManager = FindObjectOfType<LivesManager>();
            Destroy(connectionsCollector.gameObject);
            Destroy(livesManager.gameObject);          
            BackToCardPhase();
        }
        public void DisableTimer()
        {
            finalRound = true;
            timerText.text = "X";

        }
        
    }
}
