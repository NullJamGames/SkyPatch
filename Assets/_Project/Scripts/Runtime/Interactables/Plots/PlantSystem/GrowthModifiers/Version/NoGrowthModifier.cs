using NJG.Runtime.Interactables;
using NJG.Runtime.PlotSystem;

namespace NJG.Runtime.PlantSystem
{
    public class NoGrowthModifier : GrowthModifier
    {
        public override float CalculateGrowth(OldPlot oldPlot)
        {
            return 0f;
        }
        
        public override string ToString()
        {
            return "No Growth";
        }
    }
}
