using System.Collections.Generic;
using NJG.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using MEC;
using NJG.Runtime.Entity;

namespace NJG.Runtime.Interactables
{
    public class WaterContainer : PickupableItem, IInteractablePickupable
    {
        [FoldoutGroup("References"), SerializeField]
        private GameObject _waterVisual;
        [FoldoutGroup("References"), SerializeField]
        private Transform _spillPoint;
        
        [FoldoutGroup("Settings"), SerializeField]
        private float _spillAngle = 45f;
        
        [FoldoutGroup("VFX"), SerializeField]
        private GameObject _splashVFXPrefab;
        [FoldoutGroup("VFX"), SerializeField]
        private float _splashVFXDuration = 1.5f;
        [FoldoutGroup("VFX"), SerializeField]
        private float _splashVFXDelay = 1f;
        [FoldoutGroup("VFX"), SerializeField]
        private LayerMask _spillableLayers;

        private CoroutineHandle _splashRoutine;
        
        public bool HasWater { get; private set; }

        private void Start()
        {
            InvokeRepeating(nameof(CheckForSpill), 0f, 0.5f);
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

            HasWater = true;
            _waterVisual.SetActive(true);
            return true;
        }

        public bool TryEmptyWater(bool shouldSplash, float splashDelay, Vector3 splashPosition)
        {
            if (!HasWater)
                return false;

            HasWater = false;
            _waterVisual.SetActive(false);

            if (shouldSplash && _splashVFXPrefab != null)
            {
                Timing.KillCoroutines(_splashRoutine);
                _splashRoutine = Timing.RunCoroutine(SplashWaterRoutine(splashPosition, splashDelay));
            }

            return true;
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
    }
}
