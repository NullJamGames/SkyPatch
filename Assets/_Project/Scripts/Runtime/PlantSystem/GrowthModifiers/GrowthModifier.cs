using NJG.Runtime.Interactables;

namespace NJG.Runtime.PlotSystem
{
    [System.Serializable]
    public abstract class GrowthModifier
    {
        public abstract float CalculateGrowth(Plot plot);
    }
}
