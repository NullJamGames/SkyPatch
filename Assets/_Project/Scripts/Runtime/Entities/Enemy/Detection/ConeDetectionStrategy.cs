using NJG.Utilities.ImprovedTimers;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class ConeDetectionStrategy : IDetectionStrategy
    {
        private readonly float _detectionAngle;
        private readonly float _detectionRadius;
        private readonly float _innerDetectionRadius;
        
        public ConeDetectionStrategy(float detectionAngle, float detectionRadius, float innerDetectionRadius)
        {
            _detectionAngle = detectionAngle;
            _detectionRadius = detectionRadius;
            _innerDetectionRadius = innerDetectionRadius;
        }

        public bool Execute(Transform target, Transform detector, CountdownTimer timer)
        {
            if (timer.IsRunning)
                return false;
            
            Vector3 directionToTarget = target.position - detector.position;
            float angleToTarget = Vector3.Angle(directionToTarget, target.forward);
            
            // If the target is not within the detection angle + outer radius (aka the cone in front of the enemy),
            // or is within the inner radius, return false
            if ((!(angleToTarget < _detectionAngle / 2f) || !(directionToTarget.magnitude < _detectionRadius))
                && !(directionToTarget.magnitude < _innerDetectionRadius))
                return false;
            
            timer.Start();
            return true;
        }
    }
}