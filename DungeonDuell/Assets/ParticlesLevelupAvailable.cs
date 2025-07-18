using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace dungeonduell
{
    public class ParticlesLevelupAvailable : MonoBehaviour, IObserver
    {
        ParticleSystem _ps;
        int _playerId;

        private void Start()
        {
            _ps = gameObject.GetComponent<ParticleSystem>();
            _playerId =
                int.Parse(GetComponentInParent<MoreMountains.TopDownEngine.Character>().PlayerID[^1].ToString()) - 1;
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
            DdCodeEventHandler.LevelUpAvailable += ParticleLevelUp;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.LevelUpAvailable -= ParticleLevelUp;
        }

        private void ParticleLevelUp(int player, int amount)
        {
            if (_playerId == player & _ps != null)
            {
                if (amount == 0)
                {
                    _ps.Stop();
                }
                else
                {
                    _ps.Play();
                }
            }
        }
    }
}