using KBCore.Refs;
using ModelShark;
using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.UI.Tooltips
{
    [RequireComponent(typeof(TooltipTrigger), typeof(Collider))]
    public class NJGTooltipTrigger : ValidatedMonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField, Self]
        private TooltipTrigger _tooltipTrigger;
        
        private ITooltipProvider _tooltipProvider;

        private void Awake()
        {
            _tooltipProvider = GetComponentInParent<ITooltipProvider>();
            if (_tooltipProvider == null)
                Debug.LogError("Tooltip provider not found in parent GameObject.");
        }

        // private void OnEnable()
        // {
        //     _tooltipProvider.OnTooltipTextChanged += UpdateTooltipText;
        // }
        //
        // private void OnDisable()
        // {
        //     if (_tooltipProvider != null)
        //         _tooltipProvider.OnTooltipTextChanged -= UpdateTooltipText;
        // }

        private void OnTriggerEnter(Collider colliderInfo)
        {
            if (!colliderInfo.TryGetComponent(out PlayerInventory playerInventory))
                return;
            
            _tooltipTrigger.isRemotelyActivated = true;
            _tooltipTrigger.staysOpen = true;
            
            UpdateTooltipText(_tooltipProvider.GetTooltipText(playerInventory));
        }

        private void OnTriggerExit(Collider colliderInfo)
        {
            if (!colliderInfo.TryGetComponent(out PlayerInventory playerInventory))
                return;
            
            _tooltipTrigger.ForceHideTooltip();
        }

        public void UpdateTooltipText(string text)
        {
            // Reset tooltip just in case.
            _tooltipTrigger.ForceHideTooltip();
            
            _tooltipTrigger.SetText("BodyText", text);
            // Popup the tooltip (Note: duration doesn't matter, since StaysOpen is True)
            _tooltipTrigger.Popup(1f, gameObject);
        }

        public void HideTooltip() => _tooltipTrigger.ForceHideTooltip();
    }
}