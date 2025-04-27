using NJG.Runtime.Entity;
using NJG.Runtime.Pickupables;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class WaterResivor : MonoBehaviour, IInteractable
    {
        public void Interact(PlayerInventory playerInventory)
        {
            if (playerInventory.Pickupable is null)
                return;

            if (playerInventory.Pickupable is Bucket bucket)
            {
                bucket.TryFillWater();
            }
        }
    }
}
