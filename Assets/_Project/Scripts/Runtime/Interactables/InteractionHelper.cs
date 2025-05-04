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
            bool playerCanPickup = playerInventory.CanPickup();
            bool interactableHasBattery = batteryInteractable.HasBattery;

            return interactableHasBattery switch
            {
                true when playerCanPickup => "Press E to take battery",
                true => "Can't take battery, hands are full...",
                false when playerHasBattery => "Press E to insert battery",
                false => "Looks like I can insert a battery here..."
            };
        }
        
        public static string GetPickupableTooltip(PlayerInventory playerInventory, PickupableItem pickupable)
        {
            bool playerHasPickupable = playerInventory.Pickupable != null;
            
            return playerHasPickupable switch
            {
                true => "Hands are full...",
                _ => "Press E to pick up"
            };
        }

        public static string GetCompostInteractableTooltip(PlayerInventory playerInventory, CompostBin compostBin)
        {
            bool playerHasCompostable = playerInventory.Pickupable is ICompostable;
            bool interactableHasCompost = compostBin.HasCompost;

            return playerHasCompostable switch
            {
                true when !interactableHasCompost => "Press E to compost",
                true => "Can't take compost, hands are full...",
                _ => interactableHasCompost ? "Press E to take compost" : "Looks like I can compost something here..."
            };
        }
        
        public static string GetPlotInteractableTooltip(PlayerInventory playerInventory, Plot plot)
        {
            bool hasWater = false;
            if (playerInventory.Pickupable is WaterContainer waterContainer)
                hasWater = waterContainer.HasWater;
            
            Plot.PlotState plotState = plot.State;

            return plotState switch
            {
                Plot.PlotState.Empty => "Press E to plant seed",
                Plot.PlotState.Growing when hasWater => "Press E to water plant",
                Plot.PlotState.Growing => "Needs water...",
                Plot.PlotState.HarvestReady when playerInventory.CanPickup() => "Press E to harvest",
                Plot.PlotState.HarvestReady => "Can't harvest, hands are full...",
                Plot.PlotState.NoHarvest => "I wonder what this does...",
                _ => "ERROR: Something went wrong, notify dev..."
            };
        }
        
        public static string GetRevivableTreeInteractableTooltip(PlayerInventory playerInventory, RevivableTree tree)
        {
            bool hasWater = false;
            bool hasCompost = false;
            switch (playerInventory.Pickupable)
            {
                case WaterContainer waterContainer:
                    hasWater = waterContainer.HasWater;
                    break;
                case Compost compost:
                    hasCompost = true;
                    break;
            }

            return tree.State switch
            {
                RevivableTree.ObjectiveState.NeedsCompost when hasCompost => "Press E to compost",
                RevivableTree.ObjectiveState.NeedsCompost => "Needs compost...",
                RevivableTree.ObjectiveState.NeedsWater when hasWater => "Press E to water",
                RevivableTree.ObjectiveState.NeedsWater => "Needs water...",
                RevivableTree.ObjectiveState.Reviving => "Reviving...",
                RevivableTree.ObjectiveState.Completed => "That's one dang healthy tree!",
                _ => "ERROR: Something went wrong, notify dev..."
            };
        }
        
        public static string GetWaterSourceTooltip(PlayerInventory playerInventory)
        {
            bool hasWater = false;
            bool hasWaterContainer = false;
            if (playerInventory.Pickupable is WaterContainer waterContainer)
            {
                hasWaterContainer = true;
                hasWater = waterContainer.HasWater;
            }

            return hasWaterContainer switch
            {
                true when hasWater => "Container is full...",
                true => "Press E to fill water",
                _ => "I bet a plant around here would love some water..."
            };
        }
    }
}