using NJG.Runtime.Entity;

namespace NJG.Runtime.Interactables
{
    public class Compost : PickupableItem, IInteractablePickupable
    {
        public void InteractWith(IInteractable interactable, PlayerInventory playerInventory)
        {
            if (interactable is ICompostReceiver compostReceiver)
                compostReceiver.ApplyCompost(this, playerInventory);
        }
    }
}