using System;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField]
        private float _groundDistance = 0.08f;
        [SerializeField]
        private float _offset = 0.1f;
        [SerializeField]
        private LayerMask _groundLayers;
        
        public bool IsGrounded { get; private set; }

        private void Update()
        {
            Vector3 position = new Vector3(transform.position.x, transform.position.y + _offset, transform.position.z);
            IsGrounded = Physics.SphereCast(position, _groundDistance, 
                Vector3.down, out _, _groundDistance, _groundLayers);
        }

        // private void OnDrawGizmosSelected()
        // {
        //     Vector3 position = new Vector3(transform.position.x, transform.position.y + _offset, transform.position.z);
        //     
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawWireSphere(position, _groundDistance);
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawRay(position, Vector3.down * _groundDistance);
        //     Gizmos.color = Color.yellow;
        //     Gizmos.DrawRay(position, Vector3.up * _groundDistance);
        // }
    }
}