using NJG.Runtime.Entity;

namespace NJG.Runtime.Interactables
{
    public interface IWaterable
    {
        public void OnWater(PlayerInventory playerInventory, WaterContainer waterContainer);
    }
}