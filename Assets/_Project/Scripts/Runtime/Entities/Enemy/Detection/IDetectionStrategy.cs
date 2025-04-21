using NJG.Utilities.ImprovedTimers;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public interface IDetectionStrategy
    {
        public bool Execute(Transform target, Transform detector, CountdownTimer timer);
    }
}