using System;
using System.Collections.Generic;
using KBCore.Refs;
using MEC;
using NJG.Runtime.Input;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace NJG.Runtime.Managers
{
    public class CameraManager : ValidatedMonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Anywhere]
        private InputReader _input;
        [BoxGroup("References"), SerializeField, Anywhere]
        private CinemachineCamera _virtualCamera;

        [BoxGroup("Settings"), SerializeField]
        private float _gain = 1f;

        private CinemachineInputAxisController _inputAxisController;
        
        // private bool _isRMBPressed;
        // private bool _cameraMovementLock;

        private void Awake()
        {
            _inputAxisController = _virtualCamera.GetComponent<CinemachineInputAxisController>();
        }

        private void Start()
        {
            SetCameraGain();
        }

        // private void OnEnable()
        // {
        //     _input.LookEvent += OnLook;
        //     _input.EnableMouseControlCamera += OnEnableMouseControlCamera;
        //     _input.DisableMouseControlCamera += OnDisableMouseControlCamera;
        // }
        //
        // private void OnDisable()
        // {
        //     _input.LookEvent -= OnLook;
        //     _input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
        //     _input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
        // }

        private void SetCameraGain()
        {
            foreach (var controller in _inputAxisController.Controllers)
            {
                controller.Input.Gain = controller.Name switch
                {
                    "Look Orbit X" => _gain,
                    "Look Orbit Y" => -_gain,
                    _ => controller.InputValue
                };
            }
        }

        // private void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
        // {
        //     Debug.Log("On Look");
        //     
        //     if (_cameraMovementLock) 
        //         return;
        //
        //     Debug.Log("1");
        //     
        //     if (isDeviceMouse && !_isRMBPressed)
        //         return;
        //     
        //     Debug.Log("2");
        //     
        //     // If the device is a mouse use fixedDeltaTime, othewise use deltaTime
        //     float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;
        //     
        //     // Set the camera axis values
        //     foreach (var controller in _inputAxisController.Controllers)
        //     {
        //         //Debug.Log(controller.Name);
        //         // if (controller.Name == "Look Orbit X")
        //         // {
        //         //     controller.Input.Gain = 0f;
        //         //
        //         // }
        //         // if (controller.Name == "Look Orbit Y")
        //         // {
        //         //     controller.Input.Gain = 0f;
        //         // }
        //         
        //         // controller.InputValue = controller.Name switch
        //         // {
        //         //     "Look Orbit X" => cameraMovement.x * _speedMultiplier * deviceMultiplier,
        //         //     "Look Orbit Y" => cameraMovement.y * _speedMultiplier * deviceMultiplier,
        //         //     _ => controller.InputValue
        //         // };
        //     }
        // }
        //
        // private void OnEnableMouseControlCamera()
        // {
        //     _isRMBPressed = true;
        //     
        //     // Lock the cursor to the center of the screen and hide it
        //     Cursor.lockState = CursorLockMode.Locked;
        //     Cursor.visible = false;
        //
        //     Timing.RunCoroutine(DisableMouseForFrame_Coroutine());
        // }
        //
        // private IEnumerator<float> DisableMouseForFrame_Coroutine()
        // {
        //     _cameraMovementLock = true;
        //     yield return Timing.WaitForOneFrame;
        //     _cameraMovementLock = false;
        // }
        //
        // private void OnDisableMouseControlCamera()
        // {
        //     _isRMBPressed = false;
        //     
        //     // Unlock the cursor and make it visable
        //     Cursor.lockState = CursorLockMode.None;
        //     Cursor.visible = true;
        //     
        //     // Reset the camera axis to prevent jumping when re-enabling mouse control
        //     foreach (var controller in _inputAxisController.Controllers)
        //     {
        //         controller.InputValue = controller.Name switch
        //         {
        //             "Look Orbit X" or "Look Orbit Y" => 0f,
        //             _ => controller.InputValue
        //         };
        //     }
        // }
    }
}