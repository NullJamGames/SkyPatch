using System;
using NJG.Utilities.ImprovedTimers;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class PlayerDetector : MonoBehaviour
    {
        [SerializeField, Tooltip("Cone in front of the enemy")] 
        private float _detectionAngle = 60f;
        [SerializeField, Tooltip("Large circle around enemy")] 
        private float _detectionRadius = 10f;
        [SerializeField, Tooltip("Small circle around enemy")] 
        private float _innerDetectionRadius = 5f;
        [SerializeField, Tooltip("Time between detections")] 
        private float _detectionCooldown = 1f;
        [SerializeField, Tooltip("Distance from enemy to player to attack")] 
        private float _attackRange = 2f;
        
        private CountdownTimer _detectionTimer;
        private IDetectionStrategy _detectionStrategy;
        
        public Transform Player { get; private set; }
        public Health PlayerHealth { get; private set; }

        private void Awake()
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
            PlayerHealth = Player.GetComponent<Health>();
        }

        private void Start()
        {
            _detectionTimer = new CountdownTimer(_detectionCooldown);
            _detectionStrategy = new ConeDetectionStrategy(_detectionAngle, _detectionRadius, _innerDetectionRadius);
        }

        private void Update() => _detectionTimer.Tick(Time.deltaTime);

        public bool CanDetectPlayer()
        {
            return _detectionTimer.IsRunning || _detectionStrategy.Execute(Player, transform, _detectionTimer);
        }

        public bool CanAttackPlayer()
        {
            Vector3 directionToTarget = Player.position - transform.position;
            return directionToTarget.magnitude <= _attackRange;
        }
        
        public void SetDetectionStrategy(IDetectionStrategy detectionStrategy) => _detectionStrategy = detectionStrategy;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            
            // Draw a sphere for the radii
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
            Gizmos.DrawWireSphere(transform.position, _innerDetectionRadius);
            
            // Calculate our cone direction
            Vector3 forwardConeDirection =
                Quaternion.Euler(0f, _detectionAngle / 2f, 0f) * transform.forward * _detectionRadius;
            Vector3 backwardConeDirection = 
                Quaternion.Euler(0f, -_detectionAngle / 2f, 0f) * transform.forward * _detectionRadius;
            
            // Draw lines to represent the cone
            Gizmos.DrawLine(transform.position, transform.position + forwardConeDirection);
            Gizmos.DrawLine(transform.position, transform.position + backwardConeDirection);
        }
    }
}