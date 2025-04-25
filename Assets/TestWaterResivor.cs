using NJG.Runtime.Entity;
using NJG.Runtime.Interactables;
using UnityEngine;

namespace NJG
{
    public class TestWaterResivor : MonoBehaviour, IInteractable
    {
        public void Interact(PlayerInventory playerInventory)
        {
            if (playerInventory.Pickupable is null)
                return;

            if (playerInventory.Pickupable is TestBucket bucket)
            {
                bucket.TryFillWater();
            }
        }
    }
}
