using System;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    public class HitPlayerEnemy : MonoBehaviour
    {
        public bool player1IsEnemy;
        private const String Player1 = "Player1";
        private const String Player2 = "Player2";

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 10)
            {
                var character = collision.gameObject.GetComponent<Character>().PlayerID;
                if (character == (player1IsEnemy ? Player1 : Player2))
                {
                    print("HitEnemy");
                    DdCodeEventHandler.Trigger_PlayersInSameRoom();
                }
            }
        }
    }
}