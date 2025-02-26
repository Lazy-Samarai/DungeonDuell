using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace dungeonduell
{
    public class CardNavigation : MonoBehaviour
    {
        public Transform handPanel; // Das UI-Panel, in dem die Karten liegen
        private int selectedIndex = 0;
        private int cardCount = 0;
        public DisplayCard currentSelectedCard;
        private bool isReady = false;

        public InputActionReference navigateAction; // Für den linken Stick / D-Pad
        public InputActionReference selectAction; // Für die Auswahl

        private void OnEnable()
        {
            navigateAction.action.performed += ctx => Navigate(ctx.ReadValue<Vector2>());
            selectAction.action.performed += ctx => SelectCard();
            cardCount = handPanel.childCount;
        }

        private void OnDisable()
        {
            navigateAction.action.performed -= ctx => Navigate(ctx.ReadValue<Vector2>());
            selectAction.action.performed -= ctx => SelectCard();
        }

        void Start()
        {
            StartCoroutine(WaitForCards());
        }

        private IEnumerator WaitForCards()
        {
            Debug.Log("Warten auf Karten...");

            while (handPanel.childCount == 0) // Warte, bis Karten erscheinen
            {
                yield return null; // Nächsten Frame abwarten
            }

            Debug.Log($" {handPanel.childCount} Karten gefunden!");
            isReady = true;
            selectedIndex = 0;
            SetActiveCard(selectedIndex);
        }

        private void Navigate(Vector2 input)
        {
            if (input.x > 0.5f) // Nach rechts navigieren
            {
                selectedIndex++;
            }
            else if (input.x < -0.5f) // Nach links navigieren
            {
                selectedIndex--;
            }

            selectedIndex = Mathf.Clamp(selectedIndex, 0, cardCount - 1);

            SetActiveCard(selectedIndex);
        }

        private void SetActiveCard(int index)
        {

            if (currentSelectedCard != null)
            {
                currentSelectedCard.SetHighlight(false); // Vorherige Karte zurücksetzen
            }

            GameObject newSelectedCardObj = handPanel.GetChild(index).gameObject;
            EventSystem.current.SetSelectedGameObject(newSelectedCardObj);

            currentSelectedCard = newSelectedCardObj.GetComponent<DisplayCard>();
            if (currentSelectedCard != null)
            {
                currentSelectedCard.SetHighlight(true); // Neue Karte hervorheben
                Debug.Log($"✨ Karte {currentSelectedCard.card.cardName} hervorgehoben!");
            }
            else
            {
                Debug.LogError("DisplayCard-Skript auf der neuen Karte nicht gefunden!");
            }
        }

        private void SelectCard()
        {
                Debug.Log("Karte ausgewählt: " + handPanel.GetChild(selectedIndex).name);
        }
    }
}
