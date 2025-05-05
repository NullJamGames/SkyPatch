using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public interface IActivatable
    {
        public void Activate(){}
        public void Deactivate(){}
    }

    [System.Serializable]
    public struct ActivatableField
    {
        [SerializeField, OnValueChanged(nameof(OnFieldChanged)), ValidateInput("@_activatable is IActivatable")]
        private MonoBehaviour _activatable;
        
        public IActivatable Activatable => _activatable as IActivatable;
        private void OnFieldChanged()
        {
            Debug.Log("check");
            if (_activatable is IActivatable)
                return;

            if (_activatable.TryGetComponent(out IActivatable activatable))
                _activatable = activatable as MonoBehaviour;
            else
                _activatable = null;
        }
    }
}