using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class PipeWaterReservoir : WaterReservoir
    {
        [FoldoutGroup("Settings/Time"), SerializeField]
        private float _fillTime = 0.5f;
        [FoldoutGroup("Settings/Time"), SerializeField]
        private float _emptyTime = 0.5f;

        private CoroutineHandle _coroutineHandle;

        private void Start()
        {
            ContainsWater = false;
            _waterVisual.SetActive(false);
        }

        public void StartRecievingPipeWater()
        {
            Timing.KillCoroutines(_coroutineHandle);
            _coroutineHandle = Timing.RunCoroutine(FillOperation());
        }

        public void StopRecievingPipeWater()
        {
            Timing.KillCoroutines(_coroutineHandle);
            _coroutineHandle = Timing.RunCoroutine(EmptyOperation());
        }

        private IEnumerator<float> FillOperation()
        {
            yield return Timing.WaitForSeconds(_fillTime);
            ContainsWater = true;
            _waterVisual.SetActive(true);
        }

        private IEnumerator<float> EmptyOperation()
        {
            yield return Timing.WaitForSeconds(_emptyTime);
            ContainsWater = false;
            _waterVisual.SetActive(false);
        }
    }
}