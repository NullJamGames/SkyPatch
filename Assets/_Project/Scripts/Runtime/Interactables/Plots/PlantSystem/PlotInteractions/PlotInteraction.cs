using NJG.Runtime.Interactables;

namespace NJG.Runtime.PlantSystem
{
    [System.Serializable]
    public abstract class PlotInteraction
    {
        public virtual bool CanInteract(OldPlot oldPlot)
        {
            return true;
        }

        public abstract void Interact(OldPlot oldPlot);
        
    }
}