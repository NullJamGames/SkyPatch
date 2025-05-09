using System.Collections.Generic;
using MEC;
using NJG.Runtime.Audio;
using NJG.Runtime.Entity;
using NJG.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.Interactables
{
    public class WaterContainer : PickupableItem, IInteractablePickupable
    {
        [FoldoutGroup("References"), SerializeField]
        private GameObject _waterVisual;
        [FoldoutGroup("References"), SerializeField]
        private Transform _spillPoint;

        [FoldoutGroup("Settings"), SerializeField]
        private bool _hasInfiniteWater;
        [FoldoutGroup("Settings"), SerializeField]
        private float _maxWaterAmount = 100;
        [FoldoutGroup("Settings"), SerializeField]
        private float _spillAngle = 45f;

        [FoldoutGroup("WaterFill"), SerializeField]
        private LayerMask _waterFillAreaLayer;

        [FoldoutGroup("VFX"), SerializeField]
        private GameObject _splashVFXPrefab;
        [FoldoutGroup("VFX"), SerializeField]
        private float _splashVFXDuration = 1.5f;
        [FoldoutGroup("VFX"), SerializeField]
        private float _splashVFXDelay = 1f;
        [FoldoutGroup("VFX"), SerializeField]
        private LayerMask _spillableLayers;

        private AudioManager _audioManager;
        private CoroutineHandle _splashRoutine;
        private float _waterAmount;

        public bool HasWater => _waterAmount > 0;

        [Inject]
        private void Construct(AudioManager audioManager) => _audioManager = audioManager;

        private void Start()
        {
            if (_hasInfiniteWater)
                TryFillWater();
            InvokeRepeating(nameof(CheckForSpill), 0f, 0.5f);
            InvokeRepeating(nameof(CheckForWaterFillArea), 0f, 0.2f);
        }

        public void InteractWith(IInteractable interactable, PlayerInventory playerInventory)
        {
            if (interactable is IWaterable waterable)
                waterable.OnWater(playerInventory, this);
            else if (interactable is IWaterSource waterSource)
                waterSource.FillWater(this);
        }

        public bool TryFillWater()
        {
            if (HasWater)
                return false;

            _waterAmount = _maxWaterAmount;
            _waterVisual.SetActive(true);
            _audioManager.PlayOneShotAndForget(_audioManager.AudioData.FillTheBucket);
            return true;
        }

        public bool TryEmptyWater(bool shouldSplash, float splashDelay, Vector3 splashPosition)
        {
            if (!HasWater)
                return false;

            if (!_hasInfiniteWater)
                _waterAmount = 0;
            _waterVisual.SetActive(false);

            _audioManager.PlayOneShotAndForget(_audioManager.AudioData.WaterPlant);

            if (shouldSplash && _splashVFXPrefab != null)
            {
                Timing.KillCoroutines(_splashRoutine);
                _splashRoutine = Timing.RunCoroutine(SplashWaterRoutine(splashPosition, splashDelay));
            }

            return true;
        }

        public void ReduceWater(float reduceAmount)
        {
            if (!HasWater)
                return;

            _waterAmount -= reduceAmount;

            if (_waterAmount <= 0)
            {
                _waterAmount = 0;
                _waterVisual.SetActive(false);
            }
        }

        private void CheckForSpill()
        {
            if (IsPickedUp || !HasWater)
                return;

            // Check how tilted the bucket is
            float tiltX = Mathf.Abs(Tools.NormalizeAngle(transform.eulerAngles.x));
            float tiltZ = Mathf.Abs(Tools.NormalizeAngle(transform.eulerAngles.z));

            // If either X or Z axis tilt exceeds spill angle then we spill water
            if (tiltX > _spillAngle || tiltZ > _spillAngle)
                TryEmptyWater(true, _splashVFXDelay, _spillPoint.position);
        }

        private IEnumerator<float> SplashWaterRoutine(Vector3 position, float delay)
        {
            yield return Timing.WaitForSeconds(delay);

            float maxDistance = 10f;
            Physics.Raycast(position, Vector3.down, out RaycastHit hit, maxDistance, _spillableLayers);
            if (hit.collider != null)
                position = hit.point;
            else
                position = new Vector3(position.x, position.y - maxDistance, position.z);

            // TODO: Add a pool manager for VFX
            GameObject splashVFX = Instantiate(_splashVFXPrefab, position, Quaternion.identity);
            Destroy(splashVFX, _splashVFXDuration);
        }

        private void CheckForWaterFillArea()
        {
            if (!IsPickedUp)
                return;

            if (Physics.CheckSphere(transform.position, 0.1f, _waterFillAreaLayer))
                TryFillWater();
        }
    }
}