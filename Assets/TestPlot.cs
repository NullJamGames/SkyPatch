using NJG.Runtime.Entity;
using NJG.Runtime.Interactables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG
{
    public class TestPlot : MonoBehaviour, IInteractable
    {
        [FoldoutGroup("References"), SerializeField]
        private GameObject _harvestReadyGO;
        [FoldoutGroup("References"), SerializeField]
        private TestHarvestedPlant _harvestedPlant;

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
