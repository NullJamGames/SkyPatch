using NJG.Runtime.Interactables;
using UnityEngine;

namespace NJG.Runtime.PlantSystem
{
    public class PlaceSeedInteraction : PlotInteraction
    {
        [SerializeField] private PlotData _plotData;

        public override bool CanInteract(Plot plot)
        {
            return _plotData != null;
        }
        
        public override void Interact(Plot plot)
        {
            plot.SetPlotData(_plotData);
        }
        
        public override string ToString()
        {
            return "Place Seed";
        }
    }
}