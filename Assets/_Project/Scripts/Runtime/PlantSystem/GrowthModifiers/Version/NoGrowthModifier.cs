using NJG.Runtime.Interactables;
using NJG.Runtime.PlotSystem;

namespace NJG.Runtime.PlantSystem
{
    public class NoGrowthModifier : GrowthModifier
    {
        public override float CalculateGrowth(Plot plot)
        {
            return 0f;
        }
        
        public override string ToString()
        {
            return "No Growth";
        }
    }
}
