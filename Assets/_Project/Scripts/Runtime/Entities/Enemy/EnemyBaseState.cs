using NJG.Utilities.PredicateStateMachines;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class EnemyBaseState : IState
    {
        protected readonly Enemy _enemy;
        protected readonly Animator _animator;
        
        protected static readonly int _idleHash = Animator.StringToHash("IdleNormal");
        protected static readonly int _walkHash = Animator.StringToHash("WalkFWD");
        protected static readonly int _runHash = Animator.StringToHash("RunFWD");
        protected static readonly int _attackHash = Animator.StringToHash("Attack01");
        protected static readonly int _dieHash = Animator.StringToHash("Die");
        
        protected const float _crossFadeDuration = 0.1f;
        
        public EnemyBaseState(Enemy enemy, Animator animator)
        {
            _enemy = enemy;
            _animator = animator;
        }
        
        public virtual void OnEnter()
        {
            // noop
        }

        public virtual void Update()
        {
            // noop
        }

        public virtual void FixedUpdate()
        {
            // noop
        }

        public virtual void OnExit()
        {
            // noop
        }
    }
}