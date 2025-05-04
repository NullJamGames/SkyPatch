using System;
using KBCore.Refs;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace NJG.Runtime.Entities
{
    public class Player : ValidatedMonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField, Self]
        private CharacterController _character;
        [FoldoutGroup("References"), SerializeField, Anywhere]
        private CharacterCamera _characterCamera;

        private void Start()
        {
            //Cursor.lockState = CursorLockMode.Locked;
            
            _characterCamera.SetFollowTransform(_character.CameraFollowPoint);
            
            _characterCamera.IgnoredColliders.Clear();
            _characterCamera.IgnoredColliders.AddRange(_character.GetComponentsInChildren<Collider>());
        }
    }
}