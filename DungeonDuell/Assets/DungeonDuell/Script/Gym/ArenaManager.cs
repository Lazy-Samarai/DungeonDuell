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
        [SerializeField] private Transform player1Transform;
        [SerializeField] private Transform player2Transform;

        public Collider2D player1Zone;
        public Collider2D player2Zone;
        public float readyCheckRadius = 2f;
        public int totalPlayersRequired = 2;
        public TMP_Text countdownText;
        public string nextSceneName = "CardPhase";
        private Coroutine countdownCoroutine;


        private void Start()
        {
            StartCoroutine(FindPlayersAtStart());
        }

        private IEnumerator FindPlayersAtStart()
        {
            while (player1Transform == null || player2Transform == null)
            {
                GameObject p1 = GameObject.FindWithTag("Player1");
                GameObject p2 = GameObject.FindWithTag("Player2");

                if (p1 != null && player1Transform == null)
                {
                    player1Transform = p1.transform;
                }

                if (p2 != null && player2Transform == null)
                {
                    player2Transform = p2.transform;
                }

                yield return new WaitForSeconds(1f); // Warte bis Spieler erscheinen
            }
        }

        private void Update()
        {
            if (player1Transform == null || player2Transform == null) return;

            bool player1InZone = player1Zone.bounds.Contains(player1Transform.transform.position);
            bool player2InZone = player2Zone.bounds.Contains(player2Transform.transform.position);

            if (player1InZone && player2InZone)
            {
                if (countdownCoroutine == null)
                {
                    countdownCoroutine = StartCoroutine(StartGameCountdown());
                }
            }
            else
            {
                if (countdownCoroutine != null)
                {
                    StopCoroutine(countdownCoroutine);
                    countdownCoroutine = null;
                    if (countdownText != null)
                    {
                        countdownText.text = "X";
                    }
                }
            }
        }

        private IEnumerator StartGameCountdown()
        {
            float countdown = 3f;
            while (countdown > 0f)
            {
                if (countdownText != null)
                {
                    countdownText.text = Mathf.Ceil(countdown).ToString();
                }

                yield return new WaitForSeconds(1f);
                countdown -= 1f;

                bool player1StillInZone = player1Zone.bounds.Contains(player1Transform.transform.position);
                bool player2StillInZone = player2Zone.bounds.Contains(player2Transform.transform.position);
                if (!player1StillInZone || !player2StillInZone)
                {
                    if (countdownText != null)
                    {
                        countdownText.text = "X";
                    }

                    yield break;
                }
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