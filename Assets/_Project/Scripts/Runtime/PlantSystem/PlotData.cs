using System.Collections.Generic;
using System.Linq;
using NJG.Runtime.PlotSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.PlantSystem
{
    [CreateAssetMenu(fileName = "PlotData", menuName = "NJG/PlotData")]
    public class PlotData : ScriptableObject
    {
        [SerializeField]
        private PlotState[] States;
        
        public PlotState GetState(int stateIndex)
        {
            return States[stateIndex];
        }

    }

    [System.Serializable]
    public class PlotState
    {   
        [FoldoutGroup("State")]
        
        [FoldoutGroup("State/Model"), SerializeField]
        private GameObject _modelPrefab;

        [FoldoutGroup("State/Growt", true), SerializeField, PropertyTooltip("Necessery growt to change state")]
        private float _necesseryGrowt = 1;
        [SerializeReference]
        [FoldoutGroup("State/Growt"), ValueDropdown("GrowtModifierList")]
        private List<GrowthModifier> _growtModifiers = new List<GrowthModifier>(){new NoGrowthModifier()};


        [FoldoutGroup("State/Interaction"), SerializeField]
        private bool _isInteractable = false;

        [FoldoutGroup("State/Interaction"), SerializeField, ShowIf("_isInteractable")]
        private string _interactionText = "press E";
        
        [SerializeReference] 
        [FoldoutGroup("State/Interaction"), ValueDropdown("PlotInteractionList"), ShowIf("_isInteractable")]
        private List<PlotInteraction> _interactions = new List<PlotInteraction>();

        private static ValueDropdownList<GrowthModifier> GrowtModifierList =>
            GrowthModifierValueDropdownList.Modifiers();
        
        private static ValueDropdownList<PlotInteraction> PlotInteractionList => 
            PlotInteractionValueDropdownList.Interactions();
        
        
        public GameObject ModelPrefab => _modelPrefab;
        
        public float NecesseryGrowt => _necesseryGrowt;
        public List<GrowthModifier> GrowtModifiers => _growtModifiers;
        
        public bool IsInteractable => _isInteractable;
        public string InteractionText => _interactionText;
        public List<PlotInteraction> Interactions => _interactions;
        
    }
}
