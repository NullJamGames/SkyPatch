using NJG.Runtime.Entity;

namespace NJG.Runtime.Interactables
{
    public class HarvestedPlant : PickupableItem, IInteractablePickupable, ICompostable
    {
        public void OnComposted() => Destroy(gameObject);

        public void InteractWith(IInteractable interactable, PlayerInventory playerInventory)
        {
            if (interactable is CompostBin bin)
                bin.Compost(playerInventory, this);
        }
    }
}