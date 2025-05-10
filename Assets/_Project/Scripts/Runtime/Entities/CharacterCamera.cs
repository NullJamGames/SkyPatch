using System.Collections.Generic;
using KBCore.Refs;
using NJG.Runtime.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.Entities
{
    public class CharacterCamera : ValidatedMonoBehaviour
    {
        private const int _maxObstructions = 32;
        [FoldoutGroup("Framing"), SerializeField]
        private Camera _camera;
        [FoldoutGroup("Framing"), SerializeField]
        private Vector2 _followPointFraming = Vector2.zero;
        [FoldoutGroup("Framing"), SerializeField]
        private float _followingSharpness = 10000f;
        [FoldoutGroup("Distance"), SerializeField]
        private float _minDistance;
        [FoldoutGroup("Distance"), SerializeField]
        private float _maxDistance = 10f;
        [FoldoutGroup("Distance"), SerializeField]
        private float _distanceMovementSpeed = 5f;
        [FoldoutGroup("Distance"), SerializeField]
        private float _distanceMovementSharpness = 10f;

        [FoldoutGroup("Rotation"), SerializeField]
        private bool _invertX;
        [FoldoutGroup("Rotation"), SerializeField]
        private bool _invertY;
        [FoldoutGroup("Rotation"), SerializeField, Range(-90f, 90f)]
        private float _defaultVerticalAngle = 20f;
        [FoldoutGroup("Rotation"), SerializeField, Range(-90f, 90f)]
        private float _minVerticalAngle = -90f;
        [FoldoutGroup("Rotation"), SerializeField, Range(-90f, 90f)]
        private float _maxVerticalAngle = 90f;
        [FoldoutGroup("Rotation"), SerializeField]
        private float _rotationSpeed = 1f;
        [FoldoutGroup("Rotation"), SerializeField]
        private float _rotationSharpness = 10000f;

        [FoldoutGroup("Obstruction"), SerializeField]
        private float _obstructionCheckRadius = 0.2f;
        [FoldoutGroup("Obstruction"), SerializeField]
        private LayerMask _obstructionLayers = -1;
        [FoldoutGroup("Obstruction"), SerializeField]
        private float _obstructionSharpness = 10000f;

        [FoldoutGroup("Scene Start"), SerializeField]
        private Transform _startLookAt;
        [FoldoutGroup("Scene Start"), SerializeField, ShowIf("_showSetStartDistance")]
        private float _startDistance;
        private readonly RaycastHit[] _obstructions = new RaycastHit[_maxObstructions];
        private float _currentDistance;
        private Vector3 _currentFollowPosition;

        private bool _distanceIsObstructed;
        private int _obstructionCount;
        private RaycastHit _obstructionHit;
        private float _obstructionTime;
        private float _targetVerticalAngle;

        [field: FoldoutGroup("Distance"), SerializeField]
        public float DefaultDistance { get; private set; } = 6f;
        [field: FoldoutGroup("Rotation"), SerializeField]
        public bool RotateWithPhysicsMover { get; private set; }
        [field: FoldoutGroup("Obstruction"), SerializeField]
        public List<Collider> IgnoredColliders { get; private set; } = new();
        private bool _showSetStartDistance => _startLookAt != null;

        public Transform Transform { get; private set; }
        public Transform FollowTransform { get; private set; }
        public Vector3 PlanarDirection { get; set; }
        public float TargetDistance { get; set; }

        private VisualSettingsManager _visualSettingsManager;

        [Inject]
        void Construct(VisualSettingsManager visualSettingsManager)
        {
            _visualSettingsManager = visualSettingsManager;
        }

        private void Awake()
        {
            Transform = transform;

            _currentDistance = DefaultDistance;
            TargetDistance = _currentDistance;

            _targetVerticalAngle = 0f;

            PlanarDirection = Vector3.forward;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_showSetStartDistance)
                DefaultDistance = _startDistance;
            else
                DefaultDistance = Mathf.Clamp(DefaultDistance, _minDistance, _maxDistance);
            _defaultVerticalAngle = Mathf.Clamp(_defaultVerticalAngle, _minVerticalAngle, _maxVerticalAngle);
        }

        public void SetFollowTransform(Transform t)
        {
            FollowTransform = t;
            if (_startLookAt != null)
                PlanarDirection = (_startLookAt.position - t.position).normalized;
            else
                PlanarDirection = FollowTransform.forward;
            _currentFollowPosition = FollowTransform.position;
        }

        public void UpdateWithInput(float deltaTime, float zoomInput, Vector3 rotationInput)
        {
            rotationInput *= _visualSettingsManager.CameraSensitivity;
            if (FollowTransform)
            {
                if (_invertX)
                    rotationInput.x *= -1f;

                if (_invertY)
                    rotationInput.y *= -1f;

                // Process rotation input
                Quaternion rotationFromInput =
                    Quaternion.Euler(FollowTransform.up * (rotationInput.x * _rotationSpeed));
                PlanarDirection = rotationFromInput * PlanarDirection;
                PlanarDirection = Vector3.Cross(FollowTransform.up, Vector3.Cross(PlanarDirection, FollowTransform.up));
                Quaternion planarRot = Quaternion.LookRotation(PlanarDirection, FollowTransform.up);

                _targetVerticalAngle -= rotationInput.y * _rotationSpeed;
                _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, _minVerticalAngle, _maxVerticalAngle);
                Quaternion verticalRot = Quaternion.Euler(_targetVerticalAngle, 0f, 0f);
                Quaternion targetRotation = Quaternion.Slerp(Transform.rotation, planarRot * verticalRot,
                    1f - Mathf.Exp(-_rotationSharpness * deltaTime));

                // Apply rotation
                Transform.rotation = targetRotation;

                // Process distance input
                if (_distanceIsObstructed && Mathf.Abs(zoomInput) > 0f)
                    TargetDistance = _currentDistance;
                TargetDistance += zoomInput * _distanceMovementSpeed;
                TargetDistance = Mathf.Clamp(TargetDistance, _minDistance, _maxDistance);

                // Find the smoothed follow position
                _currentFollowPosition = Vector3.Lerp(_currentFollowPosition, FollowTransform.position,
                    1f - Mathf.Exp(-_followingSharpness * deltaTime));

                // Handle obstructions
                RaycastHit closestHit = new();
                closestHit.distance = Mathf.Infinity;
                _obstructionCount = Physics.SphereCastNonAlloc(_currentFollowPosition, _obstructionCheckRadius,
                    -Transform.forward, _obstructions, TargetDistance, _obstructionLayers,
                    QueryTriggerInteraction.Ignore);
                for (int i = 0; i < _obstructionCount; i++)
                {
                    bool isIgnored = false;
                    for (int j = 0; j < IgnoredColliders.Count; j++)
                        if (IgnoredColliders[j] == _obstructions[i].collider)
                        {
                            isIgnored = true;
                            break;
                        }

                    for (int j = 0; j < IgnoredColliders.Count; j++)
                        if (IgnoredColliders[j] == _obstructions[i].collider)
                        {
                            isIgnored = true;
                            break;
                        }

                    if (!isIgnored && _obstructions[i].distance < closestHit.distance && _obstructions[i].distance > 0f)
                        closestHit = _obstructions[i];
                }

                // If obstructions detected
                if (closestHit.distance < Mathf.Infinity)
                {
                    _distanceIsObstructed = true;
                    _currentDistance = Mathf.Lerp(_currentDistance, closestHit.distance,
                        1f - Mathf.Exp(-_obstructionSharpness * deltaTime));
                }
                // If no obstruction
                else
                {
                    _distanceIsObstructed = false;
                    _currentDistance = Mathf.Lerp(_currentDistance, TargetDistance,
                        1f - Mathf.Exp(-_distanceMovementSharpness * deltaTime));
                }

                // Find the smoothed camera orbit position
                Vector3 targetPosition = _currentFollowPosition - targetRotation * Vector3.forward * _currentDistance;

                // Handle framing
                targetPosition += Transform.right * _followPointFraming.x;
                targetPosition += Transform.up * _followPointFraming.y;

                // Apply position
                Transform.position = targetPosition;
            }
        }
    }
}