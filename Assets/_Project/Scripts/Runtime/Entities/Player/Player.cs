using System;
using KBCore.Refs;
using KinematicCharacterController;
using NJG.Runtime.Input;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace NJG.Runtime.Entities
{
    public class Player : ValidatedMonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField, Self]
        private CharacterController _character;
        [FoldoutGroup("References"), SerializeField, Anywhere]
        private CharacterCamera _characterCamera;
        [FoldoutGroup("References"), SerializeField, Anywhere]
        private InputReader _input;

        private void Start()
        {
            //Cursor.lockState = CursorLockMode.Locked;
            _input.EnablePlayerActions();
            
            _characterCamera.SetFollowTransform(_character.CameraFollowPoint);
            
            _characterCamera.IgnoredColliders.Clear();
            _characterCamera.IgnoredColliders.AddRange(_character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            // TODO: Temp for testing...
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            
            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            if (_characterCamera.RotateWithPhysicsMover && _character.Motor.AttachedRigidbody != null)
            {
                _characterCamera.PlanarDirection = _character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * _characterCamera.PlanarDirection;
                _characterCamera.PlanarDirection = Vector3.ProjectOnPlane(_characterCamera.PlanarDirection, _character.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }

        private void OnDisable()
        {
            if (_input != null)
                _input.DisablePlayerActions();
        }

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = _input.LookDirection.y; //Input.GetAxisRaw(MouseYInput);
            float mouseLookAxisRight = _input.LookDirection.x; //Input.GetAxisRaw(MouseXInput);
            Vector3 lookInputVector = new (mouseLookAxisRight, mouseLookAxisUp, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            // TODO: TEST THIS WITH OTHER DIRECTION...
            float scrollInput = -_input.ZoomDirection.y; //-Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

            // Apply inputs to the camera
            _characterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

            // Handle toggling zoom level
            if (UnityEngine.Input.GetMouseButtonDown(1))
            {
                _characterCamera.TargetDistance = (_characterCamera.TargetDistance == 0f) ? _characterCamera.DefaultDistance : 0f;
            }
        }
        
        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new ();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = _input.MoveDirection.y; //Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = _input.MoveDirection.x; //Input.GetAxisRaw(HorizontalInput);
            characterInputs.CameraRotation = _characterCamera.Transform.rotation;
            characterInputs.JumpDown = _input.IsJumpKeyPressed;
            characterInputs.CrouchDown = _input.IsCrouchKeyPressed; // Input.GetKeyDown(KeyCode.C);
            characterInputs.CrouchUp = _input.WasCrouchKeyReleased; // Input.GetKeyUp(KeyCode.C);

            // Apply inputs to character
            _character.SetInputs(ref characterInputs);
        }
    }
}