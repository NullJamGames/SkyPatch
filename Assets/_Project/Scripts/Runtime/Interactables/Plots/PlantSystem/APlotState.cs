using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.PlantSystem
{
    [CreateAssetMenu(fileName = "PlotState", menuName = "NJG/PlotState")]
    public class APlotState : ScriptableObject
    {
        [SerializeField]
        private PlotState _state;
        
        public PlotState State => _state;

    }
}