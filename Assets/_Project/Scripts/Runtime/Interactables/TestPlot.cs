using NJG.Runtime.Entity;
using NJG.Runtime.Pickupables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class TestPlot : MonoBehaviour, IInteractable
    {
        [FoldoutGroup("References"), SerializeField]
        private GameObject _harvestReadyGO;
        [FoldoutGroup("References"), SerializeField]
        private HarvestedPlant _harvestedPlant;

        private bool _wasHarvested;
        
        public void Interact(PlayerInventory playerInventory)
        {
            if (_wasHarvested)
                return;
            
            if (playerInventory.TryGivePickupable(_harvestedPlant))
            {
                _harvestedPlant.gameObject.SetActive(true);
                _harvestedPlant = null;
                _harvestReadyGO.SetActive(false);
                _wasHarvested = true;
            }
        }
    }
}
