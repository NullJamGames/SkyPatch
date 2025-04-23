using NJG.Runtime.Interactables;

namespace NJG.Runtime.PlantSystem
{
    public class HarvestInteraction : PlotInteraction
    {
        public override void Interact(Plot plot)
        {
            plot.OnHarvest();
        }
        
        public override string ToString()
        {
            return "Harvest";
        }
    }
}