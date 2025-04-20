using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace dungeonduell
{
    public class TestControllMouseOver : MonoBehaviour
    {
        public const float speed = 5000f;
        public PlayerInput playerInput; // Reference to PlayerInput component
        public RectTransform cursorVisual; // The visual representation of the cursor
        public Canvas canvas; // Reference to the UI canvas

        public bool ON;

        public Image image;

        private Vector2 cursorPosition;
        private VirtualMouseInput virtualMouse;

        // Start is called before the first frame update
        private void Start()
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
                var input = context.ReadValue<Vector2>();
                cursorPosition += input * Time.deltaTime * speed;

                if (cursorVisual != null) cursorVisual.anchoredPosition = cursorPosition;

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