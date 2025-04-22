using System;
using NJG.Runtime.PlantSystem;
using NJG.Utilities.ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class Plot : MonoBehaviour, IInteractable
    {
        private PlotData _plotData;
        
        [FoldoutGroup("SO Dependencies"), SerializeField]
        private APlotState _emptyPlotStateSO;
        
        
        public bool IsDaytime { get; private set; }
        public event Action OnHarvested;

        public bool HasCompost => _compostAmount > 0;
        
        private const float _growtCalculationTime = 0.25f;
        private CountdownTimer _growTimer;
        
        private PlotState _state;
        private int _stateIndex = -1;
        private PlotState _emptyState;
        
        private float _growtAmount;
        private GameObject _model;
        
        private float _compostAmount;
        
        private void Update()
        {
            _growTimer?.Tick(Time.deltaTime);
        }
        
        public void Interact()
        {
            if(!_state.IsInteractable)
                return;
            
            foreach (var plotInteraction in _state.Interactions)
                if (!plotInteraction.CanInteract(this))
                {
                    Debug.Log("Cant interact");
                    return;
                }
            
            foreach (var plotInteraction in _state.Interactions)
                plotInteraction.Interact(this);
        }
        
        public void ChangeState(int newStateIndex)
        {
            if (_state != null) 
                ExitState();
            EnterState(newStateIndex);
        }

        public void SetPlotData(PlotData plotData)
        {
            _plotData = plotData;
        }
        
        public void Initialize()
        {
            _growTimer = new CountdownTimer(_growtCalculationTime);
            _growTimer.OnTimerStop += CalculatePlantGrowt;
            _growTimer.Start();
            
            _emptyState = _emptyPlotStateSO.State;
            ChangeState(-1);
        }

        public void SetDayTime(bool isDayTime)
        {
            IsDaytime = isDayTime;
        }

        public void OnHarvest()
        {
            OnHarvested?.Invoke();
        }

        public void AddCompost(float amount)
        {
            _compostAmount += amount;
        }
        
        private void CalculatePlantGrowt()
        {
            _growTimer.Reset();
            _growTimer.Start();
            
            float growtAmount = 1;

            foreach (var VARIABLE in _state.GrowtModifiers)
                growtAmount *= VARIABLE.CalculateGrowth(this);
            
            _growtAmount += growtAmount * _growtCalculationTime;

            _compostAmount -= _growtCalculationTime;
            
            if(_growtAmount >_state.NecesseryGrowt)
                ChangeState(_stateIndex + 1);
        }
        

        private void ExitState()
        {
            if(_model != null)
                Destroy(_model);
        }

        private void EnterState(int newStateIndex)
        {
            _stateIndex = newStateIndex;

            if (_stateIndex == -1)
                _state = _emptyState;
            else
                _state = _plotData.GetState(_stateIndex);
            
            print("state : " + _stateIndex);
            
            _growtAmount = 0;
            
            if(_state.ModelPrefab != null)
                _model = Instantiate(_state.ModelPrefab, transform);
        }
    }
}