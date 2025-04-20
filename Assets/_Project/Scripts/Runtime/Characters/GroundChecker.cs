using UnityEngine;

namespace NJG.Runtime.Characters
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField]
        private float _groundDistance = 0.08f;
        [SerializeField]
        private LayerMask _groundLayers;
        
        public bool IsGrounded { get; private set; }

        private void Update()
        {
            IsGrounded = Physics.SphereCast(transform.position, _groundDistance, 
                Vector3.down, out _, _groundDistance, _groundLayers);
        }
    }
}