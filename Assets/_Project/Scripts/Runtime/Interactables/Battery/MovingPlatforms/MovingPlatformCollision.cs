using System.Collections.Generic;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class MovingPlatformCollision : MonoBehaviour
    {
        private float _platformBoundOffset = 0.1f;
        private Collider _collider;
        private List<IPlatformRider> _riders = new ();

        private void Awake() => _collider = GetComponent<Collider>();

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent(out IPlatformRider rider))
            {
                Bounds bounds = other.collider.bounds;
                float bottomY = bounds.min.y;
                float platformTopY = _collider.bounds.max.y;
                
                if (bottomY < platformTopY - _platformBoundOffset)
                    return;

                if (!_riders.Contains(rider))
                {
                    _riders.Add(rider);
                    rider.AttachToPlatform(transform);
                }
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.TryGetComponent(out IPlatformRider rider))
            {
                if (_riders.Contains(rider))
                {
                    _riders.Remove(rider);
                    rider.DetachFromPlatform();
                }
            }
        }
    }
}