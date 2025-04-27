using NJG.Runtime.Entity;

namespace NJG.Runtime.Interactables
{
    public interface IBatteryReceiver
    {
        public bool TryInsertBattery(Battery battery, PlayerInventory playerInventory);
    }
}