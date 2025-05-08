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
    public class RevivableTree : MonoBehaviour, IInteractable, IWaterable, ICompostReceiver
    {
        public enum ObjectiveState { NeedsCompost, NeedsWater, Reviving, Completed }

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

        private AudioManager _audioManager;

        private CoroutineHandle _reviveRoutine;
        public ObjectiveState State { get; private set; } = ObjectiveState.NeedsCompost;
        public Transform Transform => transform;

        public event Action<string> OnTooltipTextChanged;

        [Inject]
        private void Construct(AudioManager audioManager) => _audioManager = audioManager;

        public void ApplyCompost(Compost compost, PlayerInventory playerInventory)
        {
            if (State != ObjectiveState.NeedsCompost)
                return;

            State = ObjectiveState.NeedsWater;
            _compostPlacedVisual.SetActive(true);
            playerInventory.DetachPickupable();
            Destroy(compost.gameObject);
            OnTooltipTextChanged?.Invoke(GetTooltipText(playerInventory));
            _audioManager.StartKeyedInstance(gameObject, _audioManager.AudioData.Plant);
        }

        public void Interact(PlayerInventory playerInventory)
        {
            InteractionHelper.TryInteract(playerInventory, this);
        }

        public string GetTooltipText(PlayerInventory playerInventory)
        {
            string tooltipText = InteractionHelper.GetRevivableTreeInteractableTooltip(playerInventory, this);
            return $"TREE\n{tooltipText}";
        }

        public void OnWater(PlayerInventory playerInventory, WaterContainer waterContainer)
        {
            if (!waterContainer.TryEmptyWater(true, _splashDelay, transform.position + _splashOffset))
                return;

            if (State != ObjectiveState.NeedsWater)
                return;

            State = ObjectiveState.Reviving;
            _audioManager.StartKeyedInstance(gameObject, _audioManager.AudioData.ReviveTree);
            if (!_reviveRoutine.IsRunning)
                _reviveRoutine = Timing.RunCoroutine(ReviveTreeRoutine(playerInventory));
        }

        private IEnumerator<float> ReviveTreeRoutine(PlayerInventory playerInventory)
        {
            yield return Timing.WaitForSeconds(_reviveDelay);

            _compostPlacedVisual.SetActive(false);
            _deadTreeVersion.SetActive(false);
            _aliveTreeVersion.SetActive(true);
            State = ObjectiveState.Completed;
            OnTooltipTextChanged?.Invoke(GetTooltipText(playerInventory));
        }
    }
}