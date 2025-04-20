using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace dungeonduell
{
    public class TestControllMouseOver : MonoBehaviour
    {
        public const float Speed = 5000f;
        public PlayerInput playerInput; // Reference to PlayerInput component
        public RectTransform cursorVisual; // The visual representation of the cursor
        public Canvas canvas; // Reference to the UI canvas

        [FormerlySerializedAs("ON")] public bool on;

        public Image image;

        private Vector2 _cursorPosition;
        private VirtualMouseInput _virtualMouse;

        // Start is called before the first frame update
        private void Start()
        {
            _virtualMouse = GetComponent<VirtualMouseInput>();
            _cursorPosition = GetComponent<RectTransform>().position;
            // InputState.Change(GetComponent<VirtualMouseInput>().virtualMouse, new Vector3());         
        }

        public void OverridingMousePostion(InputAction.CallbackContext context)
        {
            _cursorPosition = GetComponent<VirtualMouseInput>().virtualMouse.position.ReadValue();
        }

        public void OnCursorMove(InputAction.CallbackContext context)
        {
            if (on)
            {
                var input = context.ReadValue<Vector2>();
                _cursorPosition += input * Time.deltaTime * Speed;

                if (cursorVisual != null) cursorVisual.anchoredPosition = _cursorPosition;

                // Update the virtual mouse position
                InputState.Change(_virtualMouse.virtualMouse.position, _cursorPosition);
            }
        }

        public void Set(bool on)
        {
            this.on = on;
            image.enabled = on;
        }
    }
}