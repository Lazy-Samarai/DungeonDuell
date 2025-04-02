using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using MoreMountains.TopDownEngine;

namespace dungeonduell
{
    public class DisplayCardController : MonoBehaviour
    {
        private PlayerInput playerInput;
        private DisplayCard displayCard;
        private CardToHand cardToHand;

        void Start()
        {
            displayCard = GetComponent<DisplayCard>();
            cardToHand = GetComponentInParent<CardToHand>();

            if (displayCard == null || cardToHand == null)
            {
                Debug.LogError($"DisplayCardController benötigt DisplayCard & CardToHand auf {gameObject.name}");
                return;
            }

            playerInput = FindCorrectPlayerInput();

            if (playerInput != null)
            {
                playerInput.actions["Submit"].performed += OnSubmitPressed;
            }
            else
            {
                Debug.LogError($"Kein passendes PlayerInput-Objekt für {gameObject.name} gefunden!");
            }
        }

        void OnDestroy()
        {
            if (playerInput != null)
            {
                playerInput.actions["Submit"].performed -= OnSubmitPressed;
            }
        }

        private void OnSubmitPressed(InputAction.CallbackContext context)
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject && IsActivePlayer())
            {
                displayCard.OnPointerClick(null);
                DDCodeEventHandler.Trigger_CardSelected(displayCard);
            }
        }

        private PlayerInput FindCorrectPlayerInput()
        {
            string neededPlayerID = IsPlayer1Card() ? "Player1" : "Player2";

            foreach (var player in FindObjectsOfType<PlayerInput>())
            {
                var playerManager = player.GetComponent<InputSystemManagerEventsBased>();
                if (playerManager != null && playerManager.PlayerID == neededPlayerID)
                {
                    return player;
                }
            }
            return null;
        }

        private bool IsPlayer1Card()
        {
            Transform parent = transform;
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
            var turnManager = FindObjectOfType<TurnManager>();
            return (turnManager.isPlayer1Turn && IsPlayer1Card()) || (!turnManager.isPlayer1Turn && !IsPlayer1Card());
        }
    }
}
