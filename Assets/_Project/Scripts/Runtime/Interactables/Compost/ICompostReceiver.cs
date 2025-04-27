using NJG.Runtime.Entity;

namespace NJG.Runtime.Interactables
{
    public interface ICompostReceiver
    {
        public void ApplyCompost(Compost compost, PlayerInventory playerInventory);
    }
}