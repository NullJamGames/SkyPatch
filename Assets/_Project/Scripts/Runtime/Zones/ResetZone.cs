using NJG.Runtime.Interfaces;
using UnityEngine;

namespace NJG.Runtime.Zones
{
    public class ResetZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IResetable resetable))
                resetable.ResetState();
        }
    }
}