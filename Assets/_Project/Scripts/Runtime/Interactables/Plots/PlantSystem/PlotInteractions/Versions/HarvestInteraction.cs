using NJG.Runtime.Interactables;

namespace NJG.Runtime.PlantSystem
{
    public class HarvestInteraction : PlotInteraction
    {
        public override void Interact(OldPlot oldPlot)
        {
            oldPlot.OnHarvest();
        }
        
        public override string ToString()
        {
            return "Harvest";
        }
    }
}