using NJG.Runtime.Interactables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class PlayerInventory : MonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField]
        private Transform _pickupParent;
        
        [FoldoutGroup("Settings"), SerializeField] 
        private LayerMask _pickupLayers;
        [FoldoutGroup("Settings"), SerializeField]  
        private float _pickupDistance = 3f;
        [FoldoutGroup("Settings"), SerializeField]  
        private Vector3 _carryOffset = new (0f, 1f, 0f);
        [FoldoutGroup("Settings"), SerializeField]  
        private float _dropCheckRadius = 0.9f;
        [FoldoutGroup("Settings"), SerializeField]  
        private LayerMask _dropPreventLayers;

        public IPickupable Pickupable { get; private set; }

        public void Pickup()
        {
            if (Pickupable != null)
                TryToDrop();
            else
                TryToPickUp();
        }

        public bool TryGetPickupable(Transform newOwner)
        {
            if (Pickupable == null)
                return false;
            
            Pickupable.Transform.SetParent(newOwner);
            Pickupable.Transform.position = newOwner.position;
            Pickupable = null;

            return true;
        }

        public bool TryGivePickupable(IPickupable pickupable)
        {
            if (Pickupable != null)
                return false;
            
            Pickupable = pickupable;
            Pickupable.Transform.SetParent(_pickupParent);
            Pickupable.Transform.position = transform.position + transform.forward + _carryOffset;

            return true;
        }
        
        private bool TryToPickUp()
        {
            Pickupable = FindClosestPickupable();

            if (Pickupable == null)
                return false;
            
            Pickupable.OnPickup();
            Pickupable.Transform.SetParent(_pickupParent);
            Pickupable.Transform.position = transform.position + _carryOffset;

            return true;
        }

        private bool TryToDrop()
        {
            if (!CanDrop())
                return false;
            
            Pickupable.Transform.SetParent(null);
            Pickupable.OnDrop();
            //Pickupable.Transform.position = transform.position + transform.forward;
            
            // TODO: Optimize... Possibly move into OnDrop
            if (Pickupable.Transform.gameObject.TryGetComponent(out Rigidbody rb))
            {
                float forceMultiplier = 2f;
                rb.AddForce(transform.forward * forceMultiplier, ForceMode.Impulse);
            }
            
            Pickupable = null;
            
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

        private IPickupable FindClosestPickupable()
        {
            Collider[] hitColliders = new Collider[10];
            int hits = Physics.OverlapSphereNonAlloc(transform.position, _pickupDistance, hitColliders, _pickupLayers);
            
            if (hits < 1)
                return null;

            IPickupable _closestPickupable = null;
            float closestDistance = float.MaxValue;
            foreach (Collider hit in hitColliders)
            {
                if (hit == null)
                    break;
                
                if (!hit.gameObject.TryGetComponent(out IPickupable pickupable))
                    continue;
                
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _closestPickupable = pickupable;
                }
            }
            
            return _closestPickupable;
        }
    }
}
