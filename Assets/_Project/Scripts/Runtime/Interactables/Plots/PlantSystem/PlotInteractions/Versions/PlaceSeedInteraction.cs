using NJG.Runtime.Interactables;
using UnityEngine;

namespace NJG.Runtime.PlantSystem
{
    public class PlaceSeedInteraction : PlotInteraction
    {
        [SerializeField] private PlotData _plotData;

        public override bool CanInteract(OldPlot oldPlot)
        {
            return _plotData != null;
        }
        
        public override void Interact(OldPlot oldPlot)
        {
            oldPlot.SetPlotData(_plotData);
        }
        
        public override string ToString()
        {
            return "Place Seed";
        }
    }
}