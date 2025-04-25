using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public interface IPickupable 
    {
        public Transform Transform { get; }

        public void OnPickup();

        public void OnDrop();
    }
}
