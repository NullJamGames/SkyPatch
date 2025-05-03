using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class PlatformStopper : MonoBehaviour
    {
        [FoldoutGroup("Platform"), SerializeField]
        private MovingPlatform _movingPlatform;
        
        private List<IPlatformRider> _stoppers = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out IPlatformRider stopper))
            {
                if(_stoppers.Contains(stopper))
                    return;
                
                _stoppers.Add(stopper);
                if(_stoppers.Count == 1)
                    _movingPlatform.AddObstacle();
            }
                
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out IPlatformRider stopper))
            {
                if(!_stoppers.Contains(stopper))
                    return;
                
                _stoppers.Remove(stopper);
                if(_stoppers.Count == 0)
                    _movingPlatform.RemoveObstacle();
            }
        }
    }
}