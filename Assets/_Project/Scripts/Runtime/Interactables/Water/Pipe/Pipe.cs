using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class Pipe : MonoBehaviour, IActivatable
    {
        [FoldoutGroup("References"), SerializeField] 
        private GameObject _pipeWater;

        [FoldoutGroup("References"), SerializeField]
        private PipeWaterReservoir _pipeWaterReservoir;
        
        [FoldoutGroup("Time"), SerializeField]
        private float _activateTime = 0.5f;
        [FoldoutGroup("Time"), SerializeField]
        private float _deactivateTime = 0.5f;
        
        private CoroutineHandle _handle;

        public void Activate()
        {
            Timing.KillCoroutines(_handle);
            _handle = Timing.RunCoroutine(ActivateOperation());
        }

        public void Deactivate()
        {
            Timing.KillCoroutines(_handle);
            _handle = Timing.RunCoroutine(DeactivateOperation());
        }
        
        private IEnumerator<float> ActivateOperation()
        {
            yield return Timing.WaitForSeconds(_activateTime);
            _pipeWater.SetActive(true);
            _pipeWaterReservoir.StartRecievingPipeWater();
        }
        
        private IEnumerator<float> DeactivateOperation()
        {
            yield return Timing.WaitForSeconds(_deactivateTime);
            _pipeWater.SetActive(false);
            _pipeWaterReservoir.StopRecievingPipeWater();
        }
    }
}