using System;
using System.Collections.Generic;
using MEC;
using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

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
        
        private enum PlotState { Empty, Growing, Ready }

        private PlotState _state = PlotState.Empty;
        private GameObject _currentVisual;
        private CoroutineHandle _growToFullRoutine;

        public void Interact(PlayerInventory playerInventory)
        {
            AssertState(playerInventory);
        }
        
        public void OnWater(WaterContainer waterContainer)
        {
            if (!_growToFullRoutine.IsRunning)
                _growToFullRoutine = Timing.RunCoroutine(GrowToFullRoutine(waterContainer));
        }
        
        private IEnumerator<float> GrowToFullRoutine(WaterContainer waterContainer)
        {
            if (!waterContainer.TryEmptyWater(true, _splashDelay, transform.position + _splashOffset))
                yield break;
            
            yield return Timing.WaitForSeconds(_growTime);
            
            _state = PlotState.Ready;
            Destroy(_currentVisual);
            _currentVisual = Instantiate(_plotData.FullyGrownPrefab, transform.position, Quaternion.identity, transform);
        }

        private void AssertState(PlayerInventory playerInventory)
        {
            switch(_state)
            {
                case PlotState.Empty:
                    EmptyPlotInteraction();
                    break;
                case PlotState.Growing:
                    GrowingPlotInteraction(playerInventory);
                    break;
                case PlotState.Ready:
                    ReadyPlotInteraction();
                    break;
            }
        }

        private void EmptyPlotInteraction()
        {
            _state = PlotState.Growing;
            _currentVisual = Instantiate(_plotData.SeedPrefab, transform.position, Quaternion.identity, transform);
        }

        private void GrowingPlotInteraction(PlayerInventory playerInventory)
        {
            InteractionHelper.TryInteract(playerInventory, this);
        }
        
        private void ReadyPlotInteraction()
        {
            _state = PlotState.Empty;
            Destroy(_currentVisual);

            float yOffset = transform.position.y + _harvestSpawnOffset;
            Vector3 spawnPosition = new (transform.position.x, yOffset, transform.position.z);
            Instantiate(_plotData.HarvestablePlantPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
