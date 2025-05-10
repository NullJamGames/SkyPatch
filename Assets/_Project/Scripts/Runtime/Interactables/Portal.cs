using NJG.Runtime.Interfaces;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class Portal : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IResetable resetable))
                resetable.ResetState();
        }
    }
}