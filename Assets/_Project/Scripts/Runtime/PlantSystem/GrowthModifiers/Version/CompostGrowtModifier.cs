using NJG.Runtime.Interactables;
using NJG.Runtime.PlotSystem;

namespace NJG.Runtime.PlantSystem
{
    public class CompostGrowtModifier : GrowthModifier
    {
        public override float CalculateGrowth(Plot plot)
        {
            if (plot.HasCompost)
                return 1.5f;
            else
                return 1;
        }
        
        public override string ToString()
        {
            return "Compost";
        }
    }
}