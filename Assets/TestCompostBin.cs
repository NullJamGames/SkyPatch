using NJG.Runtime.Entity;
using NJG.Runtime.Interactables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG
{
    public class TestCompostBin : MonoBehaviour, IInteractable
    {
        [FoldoutGroup("References"), SerializeField]
        private TestCompost _testCompostPrefab;
        [FoldoutGroup("References"), SerializeField]
        private Transform _compostHolder;
        
        private TestCompost _testCompost;
        
        public void Interact(PlayerInventory playerInventory)
        {
            if (_testCompost != null)
            {
                if (playerInventory.TryGivePickupable(_testCompost))
                {
                    _testCompost = null;
                }

                return;
            }
            
            if (playerInventory.Pickupable is null)
                return;

            if (playerInventory.Pickupable is TestHarvestedPlant plant)
            {
                if (playerInventory.TryGetPickupable(_compostHolder))
                {
                    Destroy(plant.gameObject);

                    _testCompost = Instantiate(_testCompostPrefab, _compostHolder.position, _compostHolder.rotation, _compostHolder);
                }
            }
        }
    }
}
