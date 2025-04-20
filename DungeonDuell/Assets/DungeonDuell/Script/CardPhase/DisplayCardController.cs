using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace dungeonduell
{
    public class DisplayCardController : MonoBehaviour
    {
        private CardToHand _cardToHand;
        private DisplayCard _displayCard;
        private PlayerInput _playerInput;

        private void Start()
        {
            _displayCard = GetComponent<DisplayCard>();
            _cardToHand = GetComponentInParent<CardToHand>();

            if (_displayCard == null || _cardToHand == null)
            {
                Debug.LogError($"DisplayCardController benoetigt DisplayCard & CardToHand auf {gameObject.name}");
                return;
            }

            _playerInput = FindCorrectPlayerInput();

            if (_playerInput != null)
                _playerInput.actions["Submit"].performed += OnSubmitPressed;
            else
                Debug.LogError($"Kein passendes PlayerInput-Objekt fuer {gameObject.name} gefunden!");
        }

        private void OnDestroy()
        {
            if (_playerInput != null) _playerInput.actions["Submit"].performed -= OnSubmitPressed;
        }

        private void OnSubmitPressed(InputAction.CallbackContext context)
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject && IsActivePlayer())
            {
                _displayCard.OnPointerClick(null);
                DdCodeEventHandler.Trigger_CardSelected(_displayCard);
            }
        }

        private PlayerInput FindCorrectPlayerInput()
        {
            var neededPlayerID = IsPlayer1Card() ? "Player1" : "Player2";

            foreach (var player in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
            {
                var playerManager = player.GetComponent<InputSystemManagerEventsBased>();
                if (playerManager != null && playerManager.PlayerID == neededPlayerID) return player;
            }

            return null;
        }

        private bool IsPlayer1Card()
        {
            var parent = transform;
            while (parent != null)
            {
                if (parent.name == "CanvasPlayer_1") return true;
                if (parent.name == "CanvasPlayer_2") return false;
                parent = parent.parent;
            }

            return true;
        }

        private bool IsActivePlayer()
        {
            var turnManager = FindFirstObjectByType<TurnManager>();
            return (turnManager.isPlayer1Turn && IsPlayer1Card()) || (!turnManager.isPlayer1Turn && !IsPlayer1Card());
        }
    }
}