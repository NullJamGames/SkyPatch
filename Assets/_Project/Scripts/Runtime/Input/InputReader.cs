using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static NJG.Runtime.Input.PlayerInputActions;

namespace NJG.Runtime.Input
{
    public interface IInputReader
    {
        public Vector2 MoveDirection { get; }
    }

    [CreateAssetMenu(fileName = "InputReader", menuName = "NJG/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions, IInputReader
    {
        public PlayerInputActions InputActions;
        public event UnityAction<Vector2> MoveEvent = delegate { };
        public event UnityAction<Vector2, bool> LookEvent = delegate { };
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };
        public event UnityAction<bool> JumpEvent = delegate { };
        public event UnityAction<bool> DashEvent = delegate { };
        public event UnityAction PickupEvent = delegate { };
        public event UnityAction InteractEvent = delegate { };

        public Vector2 MoveDirection => InputActions.Player.Move.ReadValue<Vector2>();
        public Vector2 LookDirection => InputActions.Player.Look.ReadValue<Vector2>();
        public Vector2 ZoomDirection => InputActions.Player.Zoom.ReadValue<Vector2>();
        public bool IsJumpKeyPressed => InputActions.Player.Jump.IsPressed();
        public bool IsCrouchKeyPressed => InputActions.Player.Crouch.IsPressed();
        public bool WasCrouchKeyReleased => InputActions.Player.Crouch.WasReleasedThisFrame();
        public bool WasInteractKeyReleased => InputActions.Player.Interact.WasReleasedThisFrame();

        public void OnEnable()
        {
            if (InputActions == null)
            {
                InputActions = new PlayerInputActions();
                InputActions.Player.SetCallbacks(this);
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent.Invoke(context.ReadValue<Vector2>());
        }

        public void OnZoom(InputAction.CallbackContext context) { }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookEvent.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }

        public void OnRotate(InputAction.CallbackContext context) { }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                InteractEvent.Invoke();
        }

        public void OnPickup(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                PickupEvent.Invoke();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    JumpEvent.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    JumpEvent.Invoke(false);
                    break;
            }
        }

        public void OnPrevious(InputAction.CallbackContext context) { }

        public void OnNext(InputAction.CallbackContext context) { }

        public void OnSprint(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    DashEvent.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    DashEvent.Invoke(false);
                    break;
            }
        }

        public void OnCrouch(InputAction.CallbackContext context) { }

        public void OnMouseControlCamera(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    EnableMouseControlCamera.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    DisableMouseControlCamera.Invoke();
                    break;
            }
        }

        public void EnablePlayerActions()
        {
            InputActions.Enable();
        }

        public void DisablePlayerActions()
        {
            InputActions.Disable();
        }

        private bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";
    }
}