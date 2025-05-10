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
        private GameObject _leavesVisuals;
        [FoldoutGroup("References"), SerializeField]
        private GameObject _compostPlacedVisual;
        
        [FoldoutGroup("Compost Shader"), SerializeField]
        private MeshRenderer _compostRenderer;
        [FoldoutGroup("Compost Shader"), SerializeField]
        private string _shaderRoughnessRef = "_Roughness";
        [FoldoutGroup("Compost Shader"), SerializeField, Range(0.1f, 0.6f)]
        private float _unwateredRoughness = 0.1f;
        [FoldoutGroup("Compost Shader"), SerializeField, Range(0.1f, 0.6f)]
        private float _wateredRoughness = 0.5f;
        
        [FoldoutGroup("Trunk Shader"), SerializeField]
        private MeshRenderer _trunkRenderer;
        [FoldoutGroup("Trunk Shader"), SerializeField]
        private string _shaderSaturationRef = "_Saturation";
        [FoldoutGroup("Trunk Shader"), SerializeField, Range(0.5f, 1f)]
        private float _deadSaturation = 0.5f;
        [FoldoutGroup("Trunk Shader"), SerializeField, Range(0.5f, 1f)]
        private float _aliveSaturation = 1f;
        
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

        private void Start()
        {
            _trunkRenderer.material.SetFloat(_shaderSaturationRef, _deadSaturation);
            _compostRenderer.material.SetFloat(_shaderRoughnessRef, _unwateredRoughness);
        }

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
            _compostRenderer.material.SetFloat(_shaderRoughnessRef, _wateredRoughness);
            _audioManager.StartKeyedInstance(gameObject, _audioManager.AudioData.ReviveTree);
            if (!_reviveRoutine.IsRunning)
                _reviveRoutine = Timing.RunCoroutine(ReviveTreeRoutine(playerInventory));
        }

        private IEnumerator<float> ReviveTreeRoutine(PlayerInventory playerInventory)
        {
            yield return Timing.WaitForSeconds(_reviveDelay);

            _leavesVisuals.SetActive(true);
            _trunkRenderer.material.SetFloat(_shaderSaturationRef, _aliveSaturation);
            State = ObjectiveState.Completed;
            OnTooltipTextChanged?.Invoke(GetTooltipText(playerInventory));
        }
    }
}