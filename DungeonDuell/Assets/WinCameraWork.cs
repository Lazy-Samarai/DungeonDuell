using System;
using Cinemachine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace dungeonduell
{
    public class WinCameraWork : MonoBehaviour, IObserver
    {
        private static readonly int WinZoom = Animator.StringToHash("WinZoom");

        void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToAllEvents();
        }

        public void SubscribeToEvents()
        {
            DdCodeEventHandler.weHaveWinner += SetCamera;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.weHaveWinner -= SetCamera;
        }

        private void SetCamera(String s)
        {
            var brains = FindObjectsByType<CinemachineBrain>(FindObjectsSortMode.None);

            foreach (CinemachineBrain brain in brains)
            {
                Animator animator = ((CinemachineVirtualCamera)brain.ActiveVirtualCamera).GetComponent<Animator>();
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                animator.SetTrigger(WinZoom);
            }
        }
    }
}