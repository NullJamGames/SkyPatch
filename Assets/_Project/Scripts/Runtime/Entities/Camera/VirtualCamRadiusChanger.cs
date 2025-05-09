using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NJG.Runtime.Entities
{
    public class VirtualCamRadiusChanger : MonoBehaviour
    {
        [FoldoutGroup("Settings"), SerializeField]
        private AnimationCurve _curve;
        [FoldoutGroup("Settings"), SerializeField]
        private float _multiplier = 1;

        [FoldoutGroup("Zoom"), SerializeField]
        private float _minZoom = 0.2f;
        [FoldoutGroup("Zoom"), SerializeField]
        private float _zoomAtStart = 1f;
        [FoldoutGroup("Zoom"), SerializeField]
        private float _smootZoomTime = 0.05f;
        [FoldoutGroup("Zoom"), SerializeField]
        private float _inputMultiplier = 0.2f;
        [FoldoutGroup("Zoom"), SerializeField]
        private InputActionReference _zoomInput;

        private CinemachineOrbitalFollow _cinemachineOrbitalFollow;
        private float _currZoom;
        private float _desiredZoom;

        private float _zoomDamp;

        private void Start()
        {
            _cinemachineOrbitalFollow = GetComponent<CinemachineOrbitalFollow>();
            _currZoom = _zoomAtStart;
            _desiredZoom = _zoomAtStart;
            SetRadius();
        }

        private void Update()
        {
            HandleZoom();
            SetRadius();
        }

        private void SetRadius()
        {
            float curr = Mathf.InverseLerp(_cinemachineOrbitalFollow.VerticalAxis.Range.x,
                _cinemachineOrbitalFollow.VerticalAxis.Range.y, _cinemachineOrbitalFollow.VerticalAxis.Value);

            _cinemachineOrbitalFollow.Radius = _curve.Evaluate(curr) * _multiplier * _currZoom;
        }

        private void HandleZoom()
        {
            float zoomInput = _zoomInput.action.ReadValue<Vector2>().y;

            _desiredZoom = Mathf.Clamp(_desiredZoom - zoomInput * _inputMultiplier, _minZoom, 1);

            _currZoom = Mathf.SmoothDamp(_currZoom, _desiredZoom, ref _zoomDamp, _smootZoomTime);
        }
    }
}