using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using TMPro;
using MoreMountains.TopDownEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.Serialization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;


namespace dungeonduell
{
    public class TurnManager : MonoBehaviour, IObserver
    {
        public LocalizeStringEvent playerTurnText;
        public LocalizeStringEvent pressAnyKeyText;
        private const int SecondsToStart = 3;
        [FormerlySerializedAs("HandPlayer1")] public CardToHand handPlayer1;
        [FormerlySerializedAs("HandPlayer2")] public CardToHand handPlayer2;

        public GameObject player1UI;
        public GameObject player2UI;

        public bool isPlayer1Turn = true;

        [FormerlySerializedAs("_playerInputs")]
        public PlayerInput[] playerInputs;

        private readonly bool[] _playerPlayedAllCards = { false, false };

        private bool _awaitingKeyPress;
        private float _timeStart;

        const string TextEntryCurrentPlayer = "Current_Player";
        private const string TextEntryMakeReady = "Make_Ready";
        private const string TextEntryEntryName = "Loading";
        private const string TextEntryStartIn = "Start_In";
        private const string TextEntryNextPlayer = "Next_Player";


        private void Start()
        {
            _timeStart = Time.time;
            InitializeTurn();

            InputUser.onChange += SetDevicesActivation;
        }

        private void Update()
        {
            if (_awaitingKeyPress && Time.time - _timeStart > 0.5f &&
                GetActivePlayerInput().actions["Submit"].WasPressedThisFrame())
                BeginPlayerActionPhase();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToAllEvents();
            try
            {
                ChangeActivateDevice(playerInputs[1].user.pairedDevices[0], true);
                ChangeActivateDevice(playerInputs[0].user.pairedDevices[0], true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SubscribeToEvents()
        {
            DdCodeEventHandler.NextPlayerTurn += EndPlayerTurn;
            DdCodeEventHandler.PlayedAllCards += SetPlayerCardsPlayed;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.NextPlayerTurn -= EndPlayerTurn;
            DdCodeEventHandler.PlayedAllCards -= SetPlayerCardsPlayed;

            InputUser.onChange -= SetDevicesActivation;
        }

        private void InitializeTurn()
        {
            _awaitingKeyPress = true;
            playerTurnText.SetEntry(TextEntryNextPlayer);
            SetPlayerInText();

            ActivateAllDevice();
            SetDevicesActivation();

            playerTurnText.gameObject.SetActive(true);
            pressAnyKeyText.gameObject.SetActive(true);
            ToggleHandVisibility(false, false);
        }

        private void SetDevicesActivation(InputUser i, InputUserChange iu, InputDevice id)
        {
            SetDevicesActivation();
        }

        private void SetDevicesActivation()
        {
            try
            {
                ChangeActivateDevice(playerInputs[1].user.pairedDevices[0], !isPlayer1Turn);
                ChangeActivateDevice(playerInputs[0].user.pairedDevices[0], isPlayer1Turn);
            }
            catch (Exception)
            {
                Debug.LogWarning("It seems not enough Input devices for all player were paired.");
            }
        }

        private void SetPlayerInText()
        {
            ((IntVariable)playerTurnText.StringReference[playerTurnText.StringReference.Keys.First()]).Value =
                isPlayer1Turn ? 1 : 2;
        }

        private void BeginPlayerActionPhase()
        {
            if (!_awaitingKeyPress) return;

            _awaitingKeyPress = false;
            UpdatePlayerTurnText();
            pressAnyKeyText.gameObject.SetActive(false);
            ToggleHandVisibility(isPlayer1Turn, !isPlayer1Turn);

            StartCoroutine(DelayedFirstSelectable());
        }


        private IEnumerator DelayedFirstSelectable()
        {
            yield return null;
            if (isPlayer1Turn) handPlayer1.FirstSelectable();
            else handPlayer2.FirstSelectable();
        }

        private void UpdatePlayerTurnText()
        {
            playerTurnText.SetEntry(TextEntryCurrentPlayer);
            SetPlayerInText();
        }

        public void EndPlayerTurn()
        {
            isPlayer1Turn = !isPlayer1Turn;

            if (_playerPlayedAllCards.All(played => played))
                InnitGameCountDown();
            else
                // Verz�gere das Initialisieren des neuen Zuges, um sicherzustellen, dass awaitingKeyPress korrekt gesetzt ist
                Invoke(nameof(InitializeTurn), 0.1f);
        }

        // Neue Methode zum Umschalten der Handkartenanzeige
        private void ToggleHandVisibility(bool showForPlayer1, bool showForPlayer2)
        {
            handPlayer1.ShowHideDeck(!showForPlayer1);
            handPlayer2.ShowHideDeck(!showForPlayer2);

            SlidePlayerSprite(player1UI, showForPlayer1);
            SlidePlayerSprite(player2UI, showForPlayer2);
        }


        public void InnitGameCountDown()
        {
            ActivateAllDevice();

            StartCoroutine(StartCountDown());
        }

        private IEnumerator StartCountDown()
        {
            pressAnyKeyText.gameObject.SetActive(true);
            pressAnyKeyText.GetComponent<TextMeshProUGUI>().fontSize *= 2;

            pressAnyKeyText.SetEntry(TextEntryMakeReady);
            yield return new WaitForSeconds(1f);
            pressAnyKeyText.SetEntry(TextEntryStartIn);

            for (var i = SecondsToStart; i > 0; i--)
            {
                ((IntVariable)pressAnyKeyText.StringReference[pressAnyKeyText.StringReference.Keys.First()]).Value = i;
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(1f);
            pressAnyKeyText.SetEntry(TextEntryEntryName);
            DdCodeEventHandler.Trigger_SceneTransition();
            
            //FindFirstObjectByType<SceneLoading>().ToTheDungeon();
        }

        private void SetPlayerCardsPlayed(bool player1)
        {
            _playerPlayedAllCards[player1 ? 0 : 1] = true;
        }

        private PlayerInput GetActivePlayerInput()
        {
            var neededID = isPlayer1Turn ? "Player1" : "Player2";
            foreach (var input in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
            {
                var manager = input.GetComponent<InputSystemManagerEventsBased>();
                if (manager != null && manager.PlayerID == neededID)
                    return input;
            }

            return null;
        }

        private void SlidePlayerSprite(GameObject uiElement, bool show, float hiddenY = -550f, float visibleY = 0f)
        {
            if (uiElement == null) return;

            var rect = uiElement.GetComponent<RectTransform>();
            if (show)
            {
                uiElement.SetActive(true);
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, hiddenY);
                rect.DOAnchorPosY(visibleY, 0.5f).SetEase(Ease.OutCubic); // DOTween
            }
            else
            {
                rect.DOAnchorPosY(hiddenY, 0.5f).SetEase(Ease.InCubic).OnComplete(() => uiElement.SetActive(false));
            }
        }

        public void ActivateAllDevice()
        {
            InputSystem.DisableDevice(Mouse.current);
            InputSystem.DisableDevice(Keyboard.current);
            foreach (PlayerInput playerInput in playerInputs)
            {
                if (playerInput.user.pairedDevices.Count > 1)
                {
                    ChangeActivateDevice(playerInput.user.pairedDevices[0], true);
                }
            }
        }

        private void ChangeActivateDevice(InputDevice device, bool on)
        {
            if (on)
            {
                // It should be noted that this could be not the best way.Usually you would call a (De)Activate
                // Input Method the player self. But in this cases the Ui is still controlled per the Controller
                // so it is done like this instead unless the 
                InputSystem.EnableDevice(device);
            }
            else
            {
                InputSystem.DisableDevice(device);
            }

            if (device is Mouse | device is Keyboard)
            {
                Cursor.lockState = !on ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = on;
                if (on) // Yes that has to be done separately  
                {
                    InputSystem.EnableDevice(Mouse.current);
                }
                else
                {
                    InputSystem.DisableDevice(Mouse.current);
                }
            }
        }
    }
}