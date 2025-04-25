using NJG.Runtime.Entity;

namespace NJG.Runtime.Interactables
{
    public interface IInteractable
    {
        public void Interact(PlayerInventory playerInventory);
    }
}