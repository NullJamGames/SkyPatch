using System;
using System.Collections.Generic;
using MEC;
using NJG.Runtime.Audio;
using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.Interactables
{
    public class Plot : MonoBehaviour, IInteractable, IWaterable
    {
        [FoldoutGroup("References"), SerializeField]
        private PlotDataSO _plotData;
        
        [FoldoutGroup("Settings"), SerializeField]
        private float _growTime = 5f;
        [FoldoutGroup("Settings"), SerializeField]
        private float _harvestSpawnOffset = 1f;
        
        [FoldoutGroup("VFX"), SerializeField]
        private float _splashDelay = 0.1f;
        [FoldoutGroup("VFX"), SerializeField]
        private Vector3 _splashOffset = Vector3.up;
        
        private GameObject _currentVisual;
        private CoroutineHandle _growToFullRoutine;
        
        public enum PlotState { Empty, Growing, HarvestReady, NoHarvest }
        public PlotState State { get; private set; } = PlotState.Empty;
        public Transform Transform => transform;
        
        public event Action<string> OnTooltipTextChanged;

        private AudioManager _audioManager;

        [Inject]
        private void Construct(AudioManager audioManager) => _audioManager = audioManager;

        public void Interact(PlayerInventory playerInventory)
        {
            AssertState(playerInventory);
        }
        
        public void OnWater(PlayerInventory playerInventory, WaterContainer waterContainer)
        {
            if (!_growToFullRoutine.IsRunning)
                _growToFullRoutine = Timing.RunCoroutine(GrowToFullRoutine(waterContainer));

        }
        
        private IEnumerator<float> GrowToFullRoutine(WaterContainer waterContainer)
        {
            if (!waterContainer.TryEmptyWater(true, _splashDelay, transform.position + _splashOffset))
                yield break;
            
            yield return Timing.WaitForSeconds(_growTime);
            
            State = _plotData.IsHarvestable ? PlotState.HarvestReady : PlotState.NoHarvest;
            Destroy(_currentVisual);
            _currentVisual = Instantiate(_plotData.FullyGrownPrefab, transform.position, transform.rotation, transform);
        }

        private void AssertState(PlayerInventory playerInventory)
        {
            switch(State)
            {
                case PlotState.Empty:
                    EmptyPlotInteraction();
                    break;
                case PlotState.Growing:
                    GrowingPlotInteraction(playerInventory);
                    break;
                case PlotState.HarvestReady:
                    ReadyPlotInteraction();
                    break;
                case PlotState.NoHarvest:
                    break;
            }
        }

        private void EmptyPlotInteraction()
        {
            State = PlotState.Growing;
            _currentVisual = Instantiate(_plotData.SeedPrefab, transform.position, Quaternion.identity, transform);
            _audioManager.PlayOneShotAndForget(_audioManager.AudioData.Plant);
        }

        private void GrowingPlotInteraction(PlayerInventory playerInventory)
        {
            InteractionHelper.TryInteract(playerInventory, this);
        }
        
        private void ReadyPlotInteraction()
        {
            State = PlotState.Empty;
            Destroy(_currentVisual);
            SpawnHarvest();
        }
        
        private void SpawnHarvest()
        {
            float yOffset = transform.position.y + _harvestSpawnOffset;
            Vector3 spawnPosition = new (transform.position.x, yOffset, transform.position.z);
            Instantiate(_plotData.HarvestablePlantPrefab, spawnPosition, Quaternion.identity);
            _audioManager.PlayOneShotAndForget(_audioManager.AudioData.PickupPlant);
        }

        public string GetTooltipText(PlayerInventory playerInventory)
        {
            string tooltipText = InteractionHelper.GetPlotInteractableTooltip(playerInventory, this);
            string plantName = State == PlotState.Empty ? "EMPTY PLOT" : _plotData.PlantName;
            
            return $"{plantName}\n{tooltipText}";
        }
    }
}
