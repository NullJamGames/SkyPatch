using NJG.Runtime.Interactables;
using UnityEngine;
using UnityEngine.Serialization;

namespace NJG.Runtime.Entity
{
    public class CarryComponent : MonoBehaviour
    {
        [SerializeField] private LayerMask _carryLayers;

        [SerializeField] private float _pickUpDistance = 3;
        
        [SerializeField] private Transform _carryParentTransform;
        
        [SerializeField] private Vector3 _carryOffset = new Vector3(0, 1, 0);
        [SerializeField] private float _dropCheckRadius = 0.9f;
        [SerializeField] private LayerMask _dropPreventLayers;

        public ICarryable Carryable { get; private set; }
        
        private float _yBeforePickUp;
        

        public bool TryToPickUp()
        {
            Carryable = FindClosestCarryable();

            if (Carryable == null)
                return false;
            
            Carryable.PickedUp();
            Carryable.Transform.SetParent(_carryParentTransform);
            Carryable.Transform.position = transform.position + transform.forward + _carryOffset;

            return true;
        }

        public bool TryToDrop()
        {
            if (!CanDrop())
                return false;
            
            Carryable.Droped();
            Carryable.Transform.SetParent(null);
            Carryable.Transform.position = transform.position + transform.forward;
            Carryable = null;
            
            return true;
        }

        private bool CanDrop()
        {
            Collider[] hitColliders = new Collider[10];
            int hits = Physics.OverlapSphereNonAlloc(transform.position + transform.forward + _carryOffset, _dropCheckRadius, hitColliders, _dropPreventLayers);
            
            if (hits < 1)
                return true;
            return false;
        }

        private ICarryable FindClosestCarryable()
        {
            Collider[] hitColliders = new Collider[10];
            int hits = Physics.OverlapSphereNonAlloc(transform.position, _pickUpDistance, hitColliders, _carryLayers);
            
            if (hits < 1)
                return null;

            ICarryable closestCarryable = null;
            float closestDistance = float.MaxValue;
            foreach (Collider hit in hitColliders)
            {
                if (hit == null)
                    break;
                
                if (!hit.gameObject.TryGetComponent(out ICarryable carryable))
                    continue;
                
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCarryable = carryable;
                }
            }
            
            return closestCarryable;
        }
    }
}
