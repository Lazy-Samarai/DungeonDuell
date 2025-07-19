using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace dungeonduell
{
    public class ParticlesLevelupAvailable : MonoBehaviour, IObserver
    {
        [SerializeField] ParticleSystem _ps;
        [SerializeField] ParticleSystem _ps2;
        int _playerId;

        private void Start()
        {
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

            DdCodeEventHandler.NewLevelUpPossible += ParticleLevelUpOnMoreLevel;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.LevelUpAvailable -= ParticleLevelUp;

            DdCodeEventHandler.NewLevelUpPossible -= ParticleLevelUpOnMoreLevel;
        }

        private void ParticleLevelUpOnMoreLevel(int player)
        {
            if (_playerId == player)
            {
                _ps2.Play();
            }
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