using NJG.Runtime.Interactables;
using UnityEngine;

namespace NJG.Runtime.PlantSystem
{
    public class ChangeStateInteraction : PlotInteraction
    {
        [SerializeField, Min(-1)] private int _nextStateIndex;
        
        public override void Interact(Plot plot)
        {
            plot.ChangeState(_nextStateIndex);
        }
        
        public override string ToString()
        {
            return "Change State";
        }
    }
}