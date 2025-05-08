using KBCore.Refs;
using KinematicCharacterController;
using NJG.Runtime.Entity;
using NJG.Runtime.Input;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Entities
{
    [RequireComponent(typeof(PlayerInteractor), typeof(PlayerInventory))]
    public class Player : ValidatedMonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField, Self]
        private CharacterController _character;
        [FoldoutGroup("References"), SerializeField, Anywhere]
        private CharacterCamera _characterCamera;
        [FoldoutGroup("References"), SerializeField, Anywhere]
        private InputReader _input;

        private PhysicsMover _physicsMover;

        public PlayerInteractor Interactor { get; private set; }
        public PlayerInventory Inventory { get; private set; }

        private void Awake()
        {
            Interactor = GetComponent<PlayerInteractor>();
            Inventory = GetComponent<PlayerInventory>();

            if (_character.Motor.AttachedRigidbody != null)
                _physicsMover = _character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>();

            InitializeController();
        }

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
            // if (UnityEngine.Input.GetMouseButtonDown(0))
            // {
            //     Cursor.lockState = CursorLockMode.Locked;
            // }

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            if (_characterCamera.RotateWithPhysicsMover && _physicsMover != null)
            {
                _characterCamera.PlanarDirection =
                    _physicsMover.RotationDeltaFromInterpolation * _characterCamera.PlanarDirection;
                _characterCamera.PlanarDirection = Vector3
                                                   .ProjectOnPlane(_characterCamera.PlanarDirection,
                                                       _character.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }

        private void OnEnable()
        {
            _input.InteractEvent += OnInteract;
            _input.PickupEvent += OnPickup;
        }

        private void OnDisable()
        {
            _input.InteractEvent -= OnInteract;
            _input.PickupEvent -= OnPickup;

            if (_input != null)
                _input.DisablePlayerActions();
        }

        // TODO: This is a temp fix..
        private void InitializeController() => _character.Initialize(this);

        private void OnInteract() => Interactor.Interact();

        private void OnPickup() => Inventory.Drop();

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = _input.LookDirection.y;
            float mouseLookAxisRight = _input.LookDirection.x;
            Vector3 lookInputVector = new(mouseLookAxisRight, mouseLookAxisUp, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
                lookInputVector = Vector3.zero;

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float scrollInput = -_input.ZoomDirection.y;
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

            // Apply inputs to the camera
            _characterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

            // Handle toggling zoom level
            // TODO: Testing temp...
            // if (UnityEngine.Input.GetMouseButtonDown(1))
            // {
            //     _characterCamera.TargetDistance = (_characterCamera.TargetDistance == 0f) ? _characterCamera.DefaultDistance : 0f;
            // }
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = _input.MoveDirection.y;
            characterInputs.MoveAxisRight = _input.MoveDirection.x;
            characterInputs.CameraRotation = _characterCamera.Transform.rotation;
            characterInputs.JumpDown = _input.IsJumpKeyPressed;
            characterInputs.CrouchDown = _input.IsCrouchKeyPressed;
            characterInputs.CrouchUp = _input.WasCrouchKeyReleased;
            characterInputs.Interact = _input.WasInteractKeyReleased;

            // Apply inputs to character
            _character.SetInputs(ref characterInputs);
        }
    }
}