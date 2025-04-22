using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public interface ICarryable 
    {
        public Transform Transform { get; }
        public virtual void PickedUp()
        {
            Transform.GetComponent<Collider>().enabled = false;
        }
        
        public virtual void Droped()
        {
            Transform.GetComponent<Collider>().enabled = true;
        }
    }
}
