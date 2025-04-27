using NJG.Runtime.Entity;

namespace NJG.Runtime.Interactables
{
    public interface IInteractablePickupable
    {
        public void InteractWith(IInteractable interactable, PlayerInventory playerInventory);
    }
}