using System;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    [Serializable]
    public class CharacterPhysics
    {
        private Rigidbody _rigidbody;
        
        public Vector3 Velocity { get; private set; }

        public CharacterPhysics(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
        }

        public void OnPhysicsUpdate()
        {
            Velocity = _rigidbody.linearVelocity;
        }
    }
}