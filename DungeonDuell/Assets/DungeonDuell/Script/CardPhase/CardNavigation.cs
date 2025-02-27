using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace dungeonduell
{
    public class CardNavigation : MonoBehaviour
    {
        public Transform handPanel;
        private int selectedIndex = 0;
        private DisplayCard currentSelectedCard;
        private bool isReady = false;

        public InputActionReference navigateAction;
        public InputActionReference selectAction;

        private void OnEnable()
        {
            navigateAction.action.performed += ctx => Navigate(ctx.ReadValue<Vector2>());
            selectAction.action.performed += ctx => SelectCard();
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

            while (GetFirstValidCardIndex() == -1) // Warte, bis Karten existieren
            {
                yield return null;
            }

            isReady = true;
            selectedIndex = GetFirstValidCardIndex();
            SetActiveCard(selectedIndex);
        }

        private void Navigate(Vector2 input)
        {
            if (!isReady) return;

            int nextIndex = selectedIndex;
            if (input.x > 0.5f) nextIndex++;
            else if (input.x < -0.5f) nextIndex--;

            int newValidIndex = GetNextValidCardIndex(nextIndex);
            if (newValidIndex != selectedIndex)
            {
                selectedIndex = newValidIndex;
                SetActiveCard(selectedIndex);
            }
        }

        private void SetActiveCard(int index)
        {
            if (!isReady || index < 0 || index >= handPanel.childCount)
            {
                Debug.LogError($"SetActiveCard: Index {index} ist ungültig. handPanel.childCount = {handPanel.childCount}");
                return;
            }

            if (currentSelectedCard != null)
            {
                currentSelectedCard.SetHighlight(false);
            }

            GameObject newSelectedCardObj = handPanel.GetChild(index).gameObject;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(newSelectedCardObj);

            currentSelectedCard = newSelectedCardObj.GetComponent<DisplayCard>();
            if (currentSelectedCard != null)
            {
                currentSelectedCard.SetHighlight(true);
            }
            else
            {
                Debug.LogError($"DisplayCard-Skript auf {newSelectedCardObj.name} nicht gefunden!");
            }
        }

        private void SelectCard()
        {
            if (!isReady) return;
            if (currentSelectedCard != null)
            {
                currentSelectedCard.HandleCardClick();
            }
        }

        private int GetFirstValidCardIndex()
        {
            for (int i = 0; i < handPanel.childCount; i++)
            {
                if (handPanel.GetChild(i).GetComponent<DisplayCard>() != null)
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetNextValidCardIndex(int index)
        {
            if (handPanel.childCount == 0) return -1;

            index = Mathf.Clamp(index, 0, handPanel.childCount - 1);
            while (handPanel.GetChild(index).GetComponent<DisplayCard>() == null)
            {
                index = (index < selectedIndex) ? Mathf.Max(0, index - 1) : Mathf.Min(handPanel.childCount - 1, index + 1);
            }
            return index;
        }
    }
}
