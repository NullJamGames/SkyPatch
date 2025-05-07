using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class WaterPipes : BatteryPowered
    {
        [FoldoutGroup("References"), SerializeField]
        private PipeVisual[] _pipes;
        [FoldoutGroup("References"), SerializeField]
        private WaterReservoir _waterReservoir;
        
        private bool _isActive;
        
        public override bool IsActive => _isActive;

        private void Start()
        {
            Deactivate();
        }

        public override void Activate()
        {
            _isActive = true;
            
            foreach (PipeVisual pipe in _pipes)
            {
                pipe.TurnOnVisuals();
            }
            
            if (_waterReservoir != null)
                _waterReservoir.SetUnlimitedWater();
        }

        public override void Deactivate()
        {
            if (_waterReservoir != null)
                _waterReservoir.SetNoWater();
            
            foreach (PipeVisual pipe in _pipes)
            {
                pipe.TurnOffVisuals();
            }
            
            _isActive = false;
        }
    }
}