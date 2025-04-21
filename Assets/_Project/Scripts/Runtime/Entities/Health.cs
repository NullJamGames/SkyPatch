using NJG.Runtime.EventChannel;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        private int _maxHealth = 100;
        [SerializeField]
        private FloatEventChannel _playerHealthChannel;
        
        private int _currentHealth;
        
        public bool IsDead => _currentHealth <= 0;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        private void Start()
        {
            PublishHealthPercentage();
        }
        
        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            PublishHealthPercentage();
        }

        private void PublishHealthPercentage()
        {
            if (_playerHealthChannel != null)
                _playerHealthChannel.Invoke(_currentHealth / (float) _maxHealth);
        }
    }
}