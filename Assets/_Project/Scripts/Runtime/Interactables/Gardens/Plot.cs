using System;
using NJG.Utilities.ImprovedTimers;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class Plot : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private GameObject _growingVarient;
        [SerializeField]
        private GameObject _harvestableVarient;
        private float _growTime;
        private CountdownTimer _growTimer;
        private bool _isDaytime;

        private PlotStage _stage = PlotStage.Empty;
        
        public event Action OnHarvested;
        
        private enum PlotStage { Empty, Planted, Harvestable }

        private void Update()
        {
            _growTimer?.Tick(Time.deltaTime);
        }

        public void Interact()
        {
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
            }
        }

        public void Initialize(float growthTime)
        {
            _growTime = growthTime;
            _growTimer = new CountdownTimer(_growTime);
            _growTimer.OnTimerStop += SetHarvestable;
        }

        public void SetDaytime()
        {
            _isDaytime = true;

            if (_stage != PlotStage.Planted)
                return;

            if (_growTimer.IsPaused)
                _growTimer.Resume();
            else
                _growTimer.Start();
        }

        public void SetNighttime()
        {
            _isDaytime = false;

            if (_growTimer.IsRunning)
                _growTimer.Pause();
        }

        private void Plant()
        {
            _stage = PlotStage.Planted;
            _growingVarient.SetActive(true);

            if (_isDaytime)
                _growTimer.Start();
        }

        private void SetHarvestable()
        {
            _stage = PlotStage.Harvestable;
            _growingVarient.SetActive(false);
            _harvestableVarient.SetActive(true);
            _growTimer.Reset();
        }

        private void Harvest()
        {
            _stage = PlotStage.Empty;
            _harvestableVarient.SetActive(false);
            OnHarvested?.Invoke();
        }
    }
}