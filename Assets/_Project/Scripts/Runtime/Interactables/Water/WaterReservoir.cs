using NJG.Runtime.Entity;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class WaterReservoir : MonoBehaviour, IInteractable, IWaterSource
    {
        public void Interact(PlayerInventory playerInventory)
        {
            InteractionHelper.TryInteract(playerInventory, this);
        }

        public void FillWater(WaterContainer waterContainer)
        {
            waterContainer.TryFillWater();
        }
    }
}
