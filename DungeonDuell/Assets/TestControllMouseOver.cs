using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace dungeonduell
{
    public class TestControllMouseOver : MonoBehaviour
    {
        public PlayerInput playerInput; // Reference to PlayerInput component
        public RectTransform cursorVisual; // The visual representation of the cursor
        public Canvas canvas; // Reference to the UI canvas

        private Vector2 cursorPosition;
        public const float speed = 5000f;
        private VirtualMouseInput virtualMouse;

        public bool ON = false;

        public UnityEngine.UI.Image image;

        // Start is called before the first frame update
        void Start()
        {
            virtualMouse = GetComponent<VirtualMouseInput>();
            cursorPosition = GetComponent<RectTransform>().position;
           // InputState.Change(GetComponent<VirtualMouseInput>().virtualMouse, new Vector3());         
        }

        public void OverridingMousePostion(InputAction.CallbackContext context)
        {
           
           cursorPosition = GetComponent<VirtualMouseInput>().virtualMouse.position.ReadValue();
        }

        public void OnCursorMove(InputAction.CallbackContext context)
        {
            if (ON)
            {
                Vector2 input = context.ReadValue<Vector2>();
                cursorPosition += input * Time.deltaTime * speed;
                print(input);

                /*
                // Clamp to screen bounds
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    cursorPosition,
                    canvas.worldCamera,
                    out var clampedPosition
                );
                */

                // cursorPosition = clampedPosition;

                if (cursorVisual != null) { cursorVisual.anchoredPosition = cursorPosition; }

                // Update the virtual mouse position
                InputState.Change(virtualMouse.virtualMouse.position, cursorPosition);
            }
   
        }
        public void Set(bool on)
        {
            ON = on;
            image.enabled = on;

        }
    }
}
