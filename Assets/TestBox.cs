using System;
using NJG.Runtime.Entity;
using NJG.Runtime.Interactables;
using NJG.Utilities.ImprovedTimers;
using UnityEngine;

namespace NJG
{
    public class TestBox : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private float _regrowTime = 5f;
        [SerializeField]
        private Material _heartMaterial;
        
        private MeshRenderer _meshRenderer;
        private CountdownTimer _timer;
        
        public bool IsHeart { get; private set; }

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            
            _timer = new CountdownTimer(_regrowTime);
            _timer.OnTimerStop += () =>
            {
                Regrow();
                _timer.Reset();
            };
        }

        private void Update()
        {
            _timer?.Tick(Time.deltaTime);
        }

        public void Interact(PlayerInventory playerInventory)
        {
            if (IsHeart)
            {
                _meshRenderer.material = _heartMaterial;
                return;
            }
            
            gameObject.SetActive(false);
            //_timer.Start();
        }

        public void Regrow()
        {
            gameObject.SetActive(true);
        }

        public void SetAsHeart()
        {
            IsHeart = true;
        }
    }
}
