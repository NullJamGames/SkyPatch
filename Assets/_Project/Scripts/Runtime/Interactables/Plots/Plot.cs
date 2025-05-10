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
        public enum PlotState { Empty, Growing, HarvestReady, NoHarvest }

        [FoldoutGroup("References"), SerializeField]
        private PlotDataSO _plotData;
        [FoldoutGroup("References"), SerializeField]
        private Transform _spawnPoint;

        [FoldoutGroup("Settings"), SerializeField]
        private float _growTime = 5f;
        [FoldoutGroup("Settings"), SerializeField]
        private float _harvestSpawnOffset = 1f;

        [FoldoutGroup("VFX"), SerializeField]
        private float _splashDelay = 0.1f;
        [FoldoutGroup("VFX"), SerializeField]
        private Vector3 _splashOffset = Vector3.up;

        private AudioManager _audioManager;

        private GameObject _currentVisual;
        private CoroutineHandle _growToFullRoutine;
        public PlotState State { get; private set; } = PlotState.Empty;
        public Transform Transform => transform;

        public event Action<string> OnTooltipTextChanged;

        [Inject]
        private void Construct(AudioManager audioManager) => _audioManager = audioManager;

        public void Interact(PlayerInventory playerInventory) => AssertState(playerInventory);

        public string GetTooltipText(PlayerInventory playerInventory)
        {
            string tooltipText = InteractionHelper.GetPlotInteractableTooltip(playerInventory, this);
            string plantName = State == PlotState.Empty ? "EMPTY PLOT" : _plotData.PlantName;

            return $"{plantName}\n{tooltipText}";
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
            _currentVisual = Instantiate(_plotData.FullyGrownPrefab, _spawnPoint.position, _spawnPoint.rotation, _spawnPoint);
        }

        private void AssertState(PlayerInventory playerInventory)
        {
            switch (State)
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
            _currentVisual = Instantiate(_plotData.SeedPrefab, _spawnPoint.position, Quaternion.identity, _spawnPoint);
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
            float yOffset = _spawnPoint.position.y + _harvestSpawnOffset;
            Vector3 spawnPosition = new(_spawnPoint.position.x, yOffset, _spawnPoint.position.z);
            Instantiate(_plotData.HarvestablePlantPrefab, spawnPosition, Quaternion.identity);
            _audioManager.PlayOneShotAndForget(_audioManager.AudioData.PickupPlant);
        }
    }
}