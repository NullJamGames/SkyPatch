using System.Collections.Generic;
using NJG.Runtime.PlotSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.PlantSystem
{
    [CreateAssetMenu(fileName = "PlotData", menuName = "NJG/PlotData")]
    public class PlotData : ScriptableObject
    {
        [FoldoutGroup("States", true), SerializeField]
        private PlotState[] States;
        
        public PlotState GetState(int stateIndex)
        {
            return States[stateIndex];
        }

    }

    [System.Serializable]
    public class PlotState
    {
        [FoldoutGroup("Model"), SerializeField]
        private GameObject _modelPrefab;

        [FoldoutGroup("Growt", true), SerializeField, PropertyTooltip("Necessery growt to change state")]
        private float _necesseryGrowt = 1;
        [SerializeReference]
        [FoldoutGroup("Growt"), ValueDropdown("GrowtModifierList")]
        private List<GrowthModifier> _growtModifiers = new List<GrowthModifier>(){new NoGrowthModifier()};


        private static ValueDropdownList<GrowthModifier> GrowtModifierList =>
            GrowthModifierValueDropdownList.Modifiers();
        
        
        public GameObject ModelPrefab => _modelPrefab;
        
        public float NecesseryGrowt => _necesseryGrowt;
        public List<GrowthModifier> GrowtModifiers => _growtModifiers;
        
    }
}
