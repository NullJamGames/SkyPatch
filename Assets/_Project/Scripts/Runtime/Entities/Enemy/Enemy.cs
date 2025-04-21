using KBCore.Refs;
using NJG.Runtime.StateSystem;
using NJG.Utilities.ImprovedTimers;
using UnityEngine;
using UnityEngine.AI;

namespace NJG.Runtime.Entity
{
    [RequireComponent(typeof(NavMeshAgent), typeof(PlayerDetector))]
    public class Enemy : Entity
    {
        [SerializeField, Self]
        private NavMeshAgent _agent;
        [SerializeField, Self]
        private PlayerDetector _playerDetector;
        [SerializeField, Child]
        private Animator _animator;

        [SerializeField]
        private float _wanderRadius = 10f;
        [SerializeField]
        private float _timeBetweenAttacks = 1f;

        private StateMachine _stateMachine;
        private CountdownTimer _attackTimer;

        private void OnValidate() => this.ValidateRefs();

        private void Start()
        {
            _attackTimer = new CountdownTimer(_timeBetweenAttacks);
            _stateMachine = new StateMachine();
            
            EnemyWanderState wanderState = new (this, _animator, _agent, _wanderRadius);
            EnemyChaseState chaseState = new (this, _animator, _agent, _playerDetector.Player);
            EnemyAttackState attackState = new (this, _animator, _agent, _playerDetector.Player);
            
            At(wanderState, chaseState, new FuncPredicate(() => _playerDetector.CanDetectPlayer()));
            At(chaseState, wanderState, new FuncPredicate(() => !_playerDetector.CanDetectPlayer()));
            At(chaseState, attackState, new FuncPredicate(() => _playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(() => !_playerDetector.CanAttackPlayer()));
            
            _stateMachine.SetState(wanderState);
        }

        private void At(IState from, IState to, IPredicate condition) =>
            _stateMachine.AddTransition(from, to, condition);
        private void Any(IState to, IPredicate condition) => _stateMachine.AddTransition(to, to, condition);

        private void Update()
        {
            _stateMachine.Update();
            _attackTimer.Tick(Time.deltaTime);
        }
        
        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }

        public void Attack()
        {
            if (_attackTimer.IsRunning)
                return;

            _attackTimer.Start();
            _playerDetector.PlayerHealth.TakeDamage(10);
        }
    }
}
