using System.Collections.Generic;
using MEC;
using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class RevivableTree : MonoBehaviour, IInteractable, IWaterable, ICompostReceiver
    {
        [FoldoutGroup("References"), SerializeField]
        private GameObject _aliveTreeVersion;
        [FoldoutGroup("References"), SerializeField]
        private GameObject _deadTreeVersion;
        [FoldoutGroup("References"), SerializeField]
        private GameObject _compostPlacedVisual;
        
        [FoldoutGroup("Settings"), SerializeField]
        private float _reviveDelay = 1f;
        
        [FoldoutGroup("VFX"), SerializeField]
        private float _splashDelay = 0.1f;
        [FoldoutGroup("VFX"), SerializeField]
        private Vector3 _splashOffset = Vector3.up;
        
        private bool _hasCompost;
        private CoroutineHandle _reviveRoutine;
        public bool IsRevived { get; private set; }
        
        public void Interact(PlayerInventory playerInventory)
        {
            InteractionHelper.TryInteract(playerInventory, this);
        }

        public void ApplyCompost(Compost compost, PlayerInventory playerInventory)
        {
            if (IsRevived || _hasCompost)
                return;
            
            _hasCompost = true;
            _compostPlacedVisual.SetActive(true);
            playerInventory.DetachPickupable();
            Destroy(compost.gameObject);
        }
        
        public void OnWater(WaterContainer waterContainer)
        {
            if (!waterContainer.TryEmptyWater(true, _splashDelay, transform.position + _splashOffset))
                return;
            
            if (IsRevived || !_hasCompost)
                return;
            
            if (!_reviveRoutine.IsRunning)
                _reviveRoutine = Timing.RunCoroutine(ReviveTreeRoutine());
        }

        private IEnumerator<float> ReviveTreeRoutine()
        {
            yield return Timing.WaitForSeconds(_reviveDelay);
            
            _compostPlacedVisual.SetActive(false);
            _deadTreeVersion.SetActive(false);
            _aliveTreeVersion.SetActive(true);
            IsRevived = true;
        }
    }
}
