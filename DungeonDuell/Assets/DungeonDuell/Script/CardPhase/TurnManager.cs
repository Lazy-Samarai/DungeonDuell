using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using MoreMountains.TopDownEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;


namespace dungeonduell
{
    public class TurnManager : MonoBehaviour, IObserver
    {
        public LocalizeStringEvent playerTurnText;

        public LocalizeStringEvent pressAnyKeyText;
        public CardToHand HandPlayer1;
        public CardToHand HandPlayer2;

        public GameObject player1UI;
        public GameObject player2UI;

        private bool awaitingKeyPress = false;
        public bool isPlayer1Turn = true;
        private float timeStart;

        private const int SecondsToStart = 3;

        private bool[] _playerPlayedAllCards = { false, false };

        public PlayerInput[] _playerInputs;

        const string TextEntryCurrentPlayer = "Current_Player";
        private const string TextEntryMakeReady = "Make_Ready";
        private const string TextEntryEntryName = "Loading";
        private const string TextEntryStartIn = "Start_In";
        private const string TextEntryNextPlayer = "Next_Player";

        void Start()
        {
            timeStart = Time.time;
            InitializeTurn();
        }

        void Update()
        {
            if (awaitingKeyPress && (Time.time - timeStart > 0.5f) &&
                GetActivePlayerInput().actions["Submit"].WasPressedThisFrame())
            {
                BeginPlayerActionPhase();
            }
        }

        void InitializeTurn()
        {
            awaitingKeyPress = true;

            playerTurnText.SetEntry(TextEntryNextPlayer);
            SetPlayerInText();

            if (isPlayer1Turn)
            {
                InputSystem.DisableDevice(_playerInputs[1].user.pairedDevices[0]);
                InputSystem.EnableDevice(_playerInputs[0].user.pairedDevices[0]);
            }
            else
            {
                InputSystem.DisableDevice(_playerInputs[0].user.pairedDevices[0]);
                InputSystem.EnableDevice(_playerInputs[1].user.pairedDevices[0]);
            }

            playerTurnText.gameObject.SetActive(true);
            pressAnyKeyText.gameObject.SetActive(true);
            ToggleHandVisibility(false, false);
        }

        private void SetPlayerInText()
        {
            ((IntVariable)playerTurnText.StringReference[playerTurnText.StringReference.Keys.First()]).Value =
                isPlayer1Turn ? 1 : 2;
        }

        void BeginPlayerActionPhase()
        {
            if (!awaitingKeyPress) return;

            awaitingKeyPress = false;
            UpdatePlayerTurnText();
            pressAnyKeyText.gameObject.SetActive(false);
            ToggleHandVisibility(isPlayer1Turn, !isPlayer1Turn);

            StartCoroutine(DelayedFirstSelectable());
        }


        private IEnumerator DelayedFirstSelectable()
        {
            yield return null;
            if (isPlayer1Turn) HandPlayer1.FirstSelectable();
            else HandPlayer2.FirstSelectable();
        }

        void UpdatePlayerTurnText()
        {
            playerTurnText.SetEntry(TextEntryCurrentPlayer);
            SetPlayerInText();
        }

        public void EndPlayerTurn()
        {
            isPlayer1Turn = !isPlayer1Turn;

            if (_playerPlayedAllCards.All(played => played == true))
            {
                InnitGameCountDown();
            }
            else
            {
                // Verz�gere das Initialisieren des neuen Zuges, um sicherzustellen, dass awaitingKeyPress korrekt gesetzt ist
                Invoke(nameof(InitializeTurn), 0.1f);
            }
        }

        // Neue Methode zum Umschalten der Handkartenanzeige
        private void ToggleHandVisibility(bool showForPlayer1, bool showForPlayer2)
        {
            HandPlayer1.ShowHideDeck(!showForPlayer1);
            HandPlayer2.ShowHideDeck(!showForPlayer2);

            SlidePlayerSprite(player1UI, showForPlayer1);
            SlidePlayerSprite(player2UI, showForPlayer2);
        }


        public void InnitGameCountDown()
        {
            InputSystem.EnableDevice(_playerInputs[0].user.pairedDevices[0]);
            InputSystem.EnableDevice(_playerInputs[1].user.pairedDevices[0]);
            StartCoroutine(StartCountDown());
        }

        IEnumerator StartCountDown()
        {
            pressAnyKeyText.gameObject.SetActive(true);
            pressAnyKeyText.GetComponent<TextMeshProUGUI>().fontSize *= 2;

            pressAnyKeyText.SetEntry(TextEntryMakeReady);
            yield return new WaitForSeconds(1f);
            pressAnyKeyText.SetEntry(TextEntryStartIn);

            for (int i = SecondsToStart; i > 0; i--)
            {
                ((IntVariable)pressAnyKeyText.StringReference[pressAnyKeyText.StringReference.Keys.First()]).Value = i;
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(1f);
            pressAnyKeyText.SetEntry(TextEntryEntryName);
            FindObjectOfType<SceneLoading>().ToTheDungeon();
        }

        private void SetPlayerCardsPlayed(bool player1)
        {
            _playerPlayedAllCards[player1 ? 0 : 1] = true;
        }

        void OnEnable() => SubscribeToEvents();
        void OnDisable() => UnsubscribeToAllEvents();

        public void SubscribeToEvents()
        {
            DDCodeEventHandler.NextPlayerTurn += EndPlayerTurn;
            DDCodeEventHandler.PlayedAllCards += SetPlayerCardsPlayed;
        }

        public void UnsubscribeToAllEvents()
        {
            DDCodeEventHandler.NextPlayerTurn -= EndPlayerTurn;
            DDCodeEventHandler.PlayedAllCards -= SetPlayerCardsPlayed;
        }

        private PlayerInput GetActivePlayerInput()
        {
            string neededID = isPlayer1Turn ? "Player1" : "Player2";
            foreach (var input in FindObjectsOfType<PlayerInput>())
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

            RectTransform rect = uiElement.GetComponent<RectTransform>();
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
    }
}