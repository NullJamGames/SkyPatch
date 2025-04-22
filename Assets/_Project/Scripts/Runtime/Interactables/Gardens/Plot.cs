using System;
using NJG.Runtime.PlantSystem;
using NJG.Utilities.ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class Plot : MonoBehaviour, IInteractable
    {
        [FoldoutGroup("SO Dependencies"), SerializeField]
        private PlotData _plotData;
        
        [FoldoutGroup("SO Dependencies"), SerializeField]
        private APlotState _emptyPlotStateSO;
        
        
        public bool IsDaytime { get; private set; }
        public event Action OnHarvested;
        
        private const float _growtCalculationTime = 0.25f;
        private CountdownTimer _growTimer;
        
        private PlotState _state;
        private int _stateIndex = -1;
        private PlotState _emptyState;
        
        private float _growtAmount;
        private GameObject _model;
        
        private void Update()
        {
            _growTimer?.Tick(Time.deltaTime);
        }
        
        private void CalculatePlantGrowt()
        {
            _growTimer.Reset();
            _growTimer.Start();
            
            float growtAmount = 1;
            
            if(_state == null)
                return;

            foreach (var VARIABLE in _state.GrowtModifiers)
                growtAmount *= VARIABLE.CalculateGrowth(this);
            
            _growtAmount += growtAmount * _growtCalculationTime;
            
            if(_growtAmount >_state.NecesseryGrowt)
                ChangeState(_stateIndex + 1);
        }
        
        private void ChangeState(int newStateIndex)
        {
            if (_state != null) 
                ExitState();
            EnterState(newStateIndex);
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

        public void Interact()
        {/*
            switch (_stage)
            {
                case PlotStage.Empty:
                    Plant();
                    break;
                case PlotStage.Planted:
                    Debug.Log("Plot is growing...");
                    break;
                case PlotStage.Harvestable:
                    Harvest();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }*/
        }

        public void Initialize()
        {
            _growTimer = new CountdownTimer(_growtCalculationTime);
            _growTimer.OnTimerStop += CalculatePlantGrowt;
            _growTimer.Start();
            
            _emptyState = _emptyPlotStateSO.State;
            ChangeState(-1);
        }

        [Button]
        public void Seed()
        {
            ChangeState(0);
        }

        public void SetDayTime(bool isDayTime)
        {
            IsDaytime = isDayTime;
        }
/*

        private void Harvest()
        {
            _stage = PlotStage.Empty;
            _harvestableVarient.SetActive(false);
            OnHarvested?.Invoke();
        }*/
    }
}