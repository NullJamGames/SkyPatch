using NJG.Utilities.PredicateStateMachines;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public abstract class BaseState : IState
    {
        protected readonly PlayerController _player;
        protected readonly Animator _animator;
        
        protected static readonly int _locomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int _jumpHash = Animator.StringToHash("Jump");
        protected static readonly int _dashHash = Animator.StringToHash("Dash");
        protected static readonly int _attackHash = Animator.StringToHash("Attack");

        protected const float _crossFadeDuration = 0.1f;

        protected BaseState(PlayerController player, Animator animator)
        {
            _player = player;
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