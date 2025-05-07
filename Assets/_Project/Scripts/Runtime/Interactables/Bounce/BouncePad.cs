using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class BouncePad : MonoBehaviour
    {
        [FoldoutGroup("Settings"), SerializeField]
        private Vector3 _force;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out ILaunchable bouncable))
            {
                Vector3 worldForce = transform.rotation * _force;
                bouncable.AddForce(worldForce);
            }
        }
    }
}