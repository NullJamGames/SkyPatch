using NJG.Runtime.Events;
using NJG.Runtime.Managers;
using NJG.Utilities.ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class SolarPanel : MonoBehaviour, IInteractable
    {
        [FoldoutGroup("References"), SerializeField]
        private GameObject _brokenVariant;
        [FoldoutGroup("References"), SerializeField]
        private GameObject _fixedVariant;
        [FoldoutGroup("References"), SerializeField]
        private IntEventChannel _energyEventChannel;
        
        [FoldoutGroup("Settings"), SerializeField]
        private int _fixCost = 10;
        [FoldoutGroup("Settings"), SerializeField]
        private int _energyPerInterval = 1;
        [FoldoutGroup("Settings"), SerializeField]
        private float _energyInterval = 1f;

        private bool _isFixed;
        private bool _isDaytime;
        private CountdownTimer _intervalTimer;

        private void Awake()
        {
            _intervalTimer = new CountdownTimer(_energyInterval);
            _intervalTimer.OnTimerStop += () =>
            {
                _energyEventChannel.Invoke(_energyPerInterval);
                _intervalTimer.Start();
            };
        }

        private void Update()
        {
            _intervalTimer?.Tick(Time.deltaTime);
        }

        public void Interact()
        {
            if (_isFixed)
                return;

            if (GameManager.Instance.HasEnoughEnergy(_fixCost))
            {
                _energyEventChannel.Invoke(-_fixCost);
                Fix();
            }
        }

        private void Fix()
        {
            _isFixed = true;
            _brokenVariant.SetActive(false);
            _fixedVariant.SetActive(true);
            
            if (_isDaytime)
                _intervalTimer.Start();
        }

        public void OnMorningTime()
        {
            _isDaytime = true;

            if (!_isFixed)
                return;
            
            _intervalTimer.Resume();
        }

        public void OnEveningTime()
        {
            _isDaytime = false;
            
            if (!_isFixed)
                return;
            
            _intervalTimer.Pause();
        }
    }
}