using NJG.Runtime.Entity;
using NJG.Runtime.UI.Tooltips;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public interface IInteractable : ITooltipProvider
    {
        public Transform Transform { get; }

        public void Interact(PlayerInventory playerInventory);
    }
}