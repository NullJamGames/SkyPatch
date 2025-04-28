using System;
using NJG.Runtime.Entity;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class WaterReservoir : MonoBehaviour, IInteractable, IWaterSource
    {
        public Transform Transform => transform;
        
        public event Action<string> OnTooltipTextChanged;
        
        public void Interact(PlayerInventory playerInventory)
        {
            InteractionHelper.TryInteract(playerInventory, this);
            OnTooltipTextChanged?.Invoke(GetTooltipText(playerInventory));
        }

        public void FillWater(WaterContainer waterContainer)
        {
            waterContainer.TryFillWater();
        }

        public string GetTooltipText(PlayerInventory playerInventory)
        {
            string tooltipText = InteractionHelper.GetWaterSourceTooltip(playerInventory);
            return $"WATER SOURCE\n{tooltipText}";
        }
    }
}
