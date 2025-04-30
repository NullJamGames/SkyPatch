using System;
using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class BouncePad : MonoBehaviour
    {
        [FoldoutGroup("Settings"), SerializeField]
        private Vector3 _force;
        
        private const string PLAYER_TAG = "Player";
        
        private void OnTriggerEnter(Collider other)
        {
            // TODO: Possibly interface this, but should be ok for now
            if (other.gameObject.TryGetComponent(out PlayerController player))
            {
                player.AddForce(_force);
            }
        }
    }
}