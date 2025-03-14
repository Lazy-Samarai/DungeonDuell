using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using MoreMountains.TopDownEngine; // Für InputSystemManagerEventsBased

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
            cardToHand = GetComponentInParent<CardToHand>(); // Holt das CardToHand-Skript

            if (displayCard == null || cardToHand == null)
            {
                Debug.LogError($"DisplayCardController benötigt DisplayCard & CardToHand auf {gameObject.name}");
                return;
            }

            // Finde das passende PlayerInput-Objekt basierend auf Canvas
            playerInput = FindCorrectPlayerInput();

            if (playerInput != null)
            {
                playerInput.actions["Submit"].performed += OnSubmitPressed;
                playerInput.actions["Back"].performed += OnBackPressed; // `Back` Action für Zurücksetzen
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
                playerInput.actions["Back"].performed -= OnBackPressed;
            }
        }

        private void OnSubmitPressed(InputAction.CallbackContext context)
        {
            // Prüft, ob diese Karte aktuell selektiert ist
            if (EventSystem.current.currentSelectedGameObject == gameObject)
            {
                displayCard.OnPointerClick(null); // Simuliert Mausklick auf die Karte
                DDCodeEventHandler.Trigger_CardSelected(displayCard);
            }
        }

        private void OnBackPressed(InputAction.CallbackContext context)
        {
            // Prüft, ob diese Karte im CardHolder liegt
            if (displayCard.transform.parent == cardToHand.cardHolder)
            {
                cardToHand.OnCardClicked(displayCard); // Führt den Rücktransfer aus
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
                    return player; // Richtige PlayerInput-Instanz gefunden
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
    }
}
