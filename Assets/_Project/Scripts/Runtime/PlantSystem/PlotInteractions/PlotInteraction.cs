using NJG.Runtime.Interactables;

namespace NJG.Runtime.PlantSystem
{
    [System.Serializable]
    public abstract class PlotInteraction
    {
        public virtual bool CanInteract(Plot plot)
        {
            return true;
        }

        public abstract void Interact(Plot plot);
        
    }
}