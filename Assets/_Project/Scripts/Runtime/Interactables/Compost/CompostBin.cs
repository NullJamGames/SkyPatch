using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class CompostBin : MonoBehaviour, IInteractable
    {
        [FoldoutGroup("References"), SerializeField]
        private Compost _testCompostPrefab;
        [FoldoutGroup("References"), SerializeField]
        private Transform _compostHolder;
        
        private Compost _testCompost;
        
        public void Interact(PlayerInventory playerInventory)
        {
            // if (_testCompost != null)
            // {
            //     if (playerInventory.TryGivePickupable(_testCompost))
            //     {
            //         _testCompost = null;
            //     }
            //
            //     return;
            // }
            //
            // if (playerInventory.Pickupable is null)
            //     return;
            //
            // if (playerInventory.Pickupable is HarvestedPlant plant)
            // {
            //     if (playerInventory.DetachPickupable(_compostHolder))
            //     {
            //         Destroy(plant.gameObject);
            //
            //         _testCompost = Instantiate(_testCompostPrefab, _compostHolder.position, _compostHolder.rotation, _compostHolder);
            //     }
            // }
        }
    }
}
