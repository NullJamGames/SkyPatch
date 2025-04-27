using NJG.Runtime.Entity;
using NJG.Runtime.Pickupables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class RevivableTree : MonoBehaviour, IInteractable
    {
        [FoldoutGroup("References"), SerializeField]
        private GameObject _aliveTreeVersion;
        [FoldoutGroup("References"), SerializeField]
        private GameObject _deadTreeVersion;
        [FoldoutGroup("References"), SerializeField]
        private GameObject _compostPlacedVisual;
        
        private bool _hasCompost;
        private bool _hasWater;
        public bool IsRevived { get; private set; }
        
        public void Interact(PlayerInventory playerInventory)
        {
            if (IsRevived)
                return;
            
            if (playerInventory.Pickupable is null)
                return;
            
            if (playerInventory.Pickupable is Compost compost)
            {
                if (playerInventory.TryGetPickupable(transform))
                {
                    Destroy(compost.gameObject);
                    _hasCompost = true;
                    _compostPlacedVisual.SetActive(true);
                    return;
                }
            }
            
            if (!_hasCompost)
                return;
            
            if (playerInventory.Pickupable is Bucket bucket)
            {
                if (bucket.TryEmptyWater())
                {
                    _hasWater = true;
                    _compostPlacedVisual.SetActive(false);
                }
            }
            
            if (!_hasWater)
                return;

            ReviveTree();
        }

        private void ReviveTree()
        {
            _deadTreeVersion.SetActive(false);
            _aliveTreeVersion.SetActive(true);
            IsRevived = true;
        }
    }
}
