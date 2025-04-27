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
        
        // ===================== TOOLTIPS =====================
        public static string GetBatteryInteractableTooltip(PlayerInventory playerInventory,
            BatteryInteractable batteryInteractable
        )
        {
            bool playerHasBattery = playerInventory.Pickupable is Battery;
            bool interactableHasBattery = batteryInteractable.HasBattery;

            return playerHasBattery switch
            {
                true when !interactableHasBattery => "Press E to insert battery",
                true => "Can't take battery, hands are full...",
                _ => interactableHasBattery ? "Press E to take battery" : "Looks like I can insert a battery here..."
            };
        }
    }
}