using System;
using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class CompostBin : MonoBehaviour, IInteractable, IGivableInteractable
    {
        [FoldoutGroup("References"), SerializeField]
        private Compost _compostPrefab;
        [FoldoutGroup("References"), SerializeField]
        private Transform _compostHolder;
        
        [FoldoutGroup("General"), SerializeField]
        private string _name = "NAME";
        
        private Compost _compost;
        
        public Transform Transform => transform;
        public bool HasCompost => _compost != null;
        
        public event Action<string> OnTooltipTextChanged;
        
        public void Interact(PlayerInventory playerInventory)
        {
            InteractionHelper.TryInteract(playerInventory, this);
        }

        public void Compost(PlayerInventory playerInventory, ICompostable compostable)
        {
            playerInventory.DetachPickupable();
            compostable.OnComposted();
            _compost = Instantiate(_compostPrefab, _compostHolder.position, _compostHolder.rotation, _compostHolder);
        }

        public bool TryGivePickupable(PlayerInventory playerInventory)
        {
            if (_compost == null || !playerInventory.CanPickup())
                return false;
            
            playerInventory.AttachPickupable(_compost);
            _compost = null;
            return true;
        }

        public string GetTooltipText(PlayerInventory playerInventory)
        {
            string tooltipText = InteractionHelper.GetCompostInteractableTooltip(playerInventory, this);
            return $"{_name}\n{tooltipText}";
        }
    }
}
