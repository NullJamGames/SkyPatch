using NJG.Runtime.Entity;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class WaterReservoir : MonoBehaviour, IInteractable
    {
        public void Interact(PlayerInventory playerInventory)
        {
            if (playerInventory.Pickupable is Bucket bucket)
            {
                bucket.TryFillWater();
            }
        }
    }
}
