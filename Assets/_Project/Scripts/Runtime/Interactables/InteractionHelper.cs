using NJG.Runtime.Entity;

namespace NJG.Runtime.Interactables
{
    public static class InteractionHelper
    {
        public static bool TryInteract(PlayerInventory playerInventory, IInteractable interactable)
        {
            if (playerInventory.Pickupable is IInteractablePickupable pickupable)
            {
                pickupable.InteractWith(interactable, playerInventory);
                return true;
            }
            else if (interactable is IGivableInteractable givable)
            {
                return givable.TryGivePickupable(playerInventory);
            }

            return false;
        }
    }
}