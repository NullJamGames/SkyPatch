using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class GroundChecker : MonoBehaviour
    {
        [FoldoutGroup("Settings"), SerializeField]
        private LayerMask _groundLayers;
        [FoldoutGroup("Settings"), SerializeField]
        private float _radius = 0.1f;
        [FoldoutGroup("Settings"), SerializeField]
        private float _checkDistance = 0.08f;
        [FoldoutGroup("Settings"), SerializeField]
        private float _yOffset = 0.1f;
        
        public bool IsGrounded { get; private set; }

        private void Update() => CheckIfGrounded();

        private void CheckIfGrounded()
        {
            Vector3 position = new (transform.position.x, transform.position.y + _yOffset, transform.position.z);
            IsGrounded = Physics.SphereCast(position, _radius, Vector3.down, out _, _checkDistance, _groundLayers);
        }

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 position = new (transform.position.x, transform.position.y + _yOffset, transform.position.z);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, _radius);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(position, Vector3.down * _checkDistance);
        }
        #endif
    }
}