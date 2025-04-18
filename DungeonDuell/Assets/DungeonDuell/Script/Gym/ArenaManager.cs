using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Unity.UI;
using TMPro;

namespace dungeonduell
{
    public class ArenaManager : MonoBehaviour
    {
        public Transform readyZone;
        public float readyCheckRadius = 2f;
        public int totalPlayersRequired = 2;
        public TextMeshPro countdownText;
        public string nextSceneName = "CardPhaseScene";

        private bool isCountingDown = false;

        private void Update()
        {
            if (!isCountingDown)
            {
                CheckReadyZone();
            }
        }

        private void CheckReadyZone()
        {
            Collider[] players = Physics.OverlapSphere(readyZone.position, readyCheckRadius);
            int readyCount = 0;
            foreach (var col in players)
            {
                if (col.CompareTag("Player"))
                {
                    readyCount++;
                }
            }

            if (readyCount >= totalPlayersRequired)
            {
                StartCoroutine(StartGameCountdown());
            }
        }

        private IEnumerator StartGameCountdown()
        {
            isCountingDown = true;

            float countdown = 3f;
            while (countdown > 0)
            {
                if (countdownText != null)
                {
                    countdownText.text = Mathf.Ceil(countdown).ToString();
                }
                yield return new WaitForSeconds(1f);
                countdown -= 1f;
            }

            if (countdownText != null)
            {
                countdownText.text = "GO!";
            }

            yield return new WaitForSeconds(1f);
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }
}
