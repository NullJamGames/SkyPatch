using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public interface IPickupable : IInteractable
    {
        public Transform Transform { get; }

        public void OnPickup();

        public void OnDrop();
    }
}