using System;
using Unity.Cinemachine;
using UnityEngine;

namespace NJG.Runtime.Entities
{
    public class VirtualCamRadiusChanger : MonoBehaviour
    {
        [SerializeField] AnimationCurve _curve;

        [SerializeField] private float _multiplier = 1;
        
        private CinemachineOrbitalFollow _cinemachineOrbitalFollow;

        private void OnEnable()
        {
            _cinemachineOrbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        }

        private void Update()
        {
            float curr = Mathf.InverseLerp(_cinemachineOrbitalFollow.VerticalAxis.Range.x, _cinemachineOrbitalFollow.VerticalAxis.Range.y, _cinemachineOrbitalFollow.VerticalAxis.Value);
            
            _cinemachineOrbitalFollow.Radius = _curve.Evaluate(curr) * _multiplier;
        }
    }
}
