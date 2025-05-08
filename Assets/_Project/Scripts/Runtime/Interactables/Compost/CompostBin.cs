using System;
using NJG.Runtime.Audio;
using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.Interactables
{
    public class CompostBin : MonoBehaviour, IInteractable, IGivableInteractable
    {
        [FoldoutGroup("References"), SerializeField]
        private Compost _compostPrefab;
        [FoldoutGroup("References"), SerializeField]
        private Transform _compostHolder;

        [FoldoutGroup("General"), SerializeField]
        private string _name = "NAME";

        private AudioManager _audioManager;
        private Compost _compost;

        public bool HasCompost => _compost != null;

        public Transform Transform => transform;

        public event Action<string> OnTooltipTextChanged;

        [Inject]
        private void Construct(AudioManager audioManager) => _audioManager = audioManager;

        public bool TryGivePickupable(PlayerInventory playerInventory)
        {
            if (_compost == null || !playerInventory.CanPickup())
                return false;

            playerInventory.AttachPickupable(_compost);
            _compost = null;
            return true;
        }

        public void Interact(PlayerInventory playerInventory)
        {
            InteractionHelper.TryInteract(playerInventory, this);
        }

        public string GetTooltipText(PlayerInventory playerInventory)
        {
            string tooltipText = InteractionHelper.GetCompostInteractableTooltip(playerInventory, this);
            return $"{_name}\n{tooltipText}";
        }

        public void Compost(PlayerInventory playerInventory, ICompostable compostable)
        {
            playerInventory.DetachPickupable();
            _audioManager.PlayOneShotAndForget(_audioManager.AudioData.CompostBin);
            compostable.OnComposted();
            _compost = Instantiate(_compostPrefab, _compostHolder.position, _compostHolder.rotation, _compostHolder);
        }
    }
}