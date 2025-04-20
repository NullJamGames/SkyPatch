using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static NJG.Runtime.Input.PlayerInputActions;

namespace NJG.Runtime.Input
{
    public interface IInputReader
    {
        public Vector2 Direction { get; }
    }
    
    [CreateAssetMenu(fileName = "InputReader", menuName = "NJG/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions, IInputReader
    {
        public event UnityAction<Vector2> MoveEvent = delegate { };
        public event UnityAction<Vector2, bool> LookEvent = delegate { };
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };
        public event UnityAction<bool> JumpEvent = delegate { };

        public PlayerInputActions InputActions;

        public Vector2 Direction => InputActions.Player.Move.ReadValue<Vector2>();
        public bool IsJumpKeyPressed => InputActions.Player.Jump.IsPressed();
        
        public void OnEnable()
        {
            if (InputActions == null)
            {
                InputActions = new PlayerInputActions();
                InputActions.Player.SetCallbacks(this);
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

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent.Invoke(context.ReadValue<Vector2>());
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookEvent.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }
        
        private bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

        public void OnRotate(InputAction.CallbackContext context) { }

        public void OnInteract(InputAction.CallbackContext context) { }

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

        public void OnSprint(InputAction.CallbackContext context) { }

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
    }
}