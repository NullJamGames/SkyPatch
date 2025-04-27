using NJG.Runtime.Entity;

namespace NJG.Runtime.Interactables
{
    public interface IGivableInteractable
    {
        public bool TryGivePickupable(PlayerInventory playerInventory);
    }
}