using NJG.Runtime.Input;
using NJG.Utilities.PredicateStateMachines;
using UnityEngine;
using CharacterController = NJG.Runtime.Entities.CharacterController;

namespace NJG.Runtime.Entity
{
    public abstract class BaseState : IState
    {
        protected const float _crossFadeDuration = 0.1f;

        protected static readonly int _locomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int _jumpHash = Animator.StringToHash("Jump");
        protected static readonly int _fallHash = Animator.StringToHash("Fall");
        protected static readonly int _dashHash = Animator.StringToHash("Dash");
        protected static readonly int _attackHash = Animator.StringToHash("Attack");
        protected static readonly int _climbHash = Animator.StringToHash("Climb");
        protected readonly Animator _animator;
        protected readonly CharacterController _character;

        protected BaseState(CharacterController character, Animator animator)
        {
            _character = character;
            _animator = animator;
        }

        public virtual void OnEnter()
        {
            // noop
        }

        public void OnUpdate(float deltaTime)
        {
            // noop
        }

        public void OnFixedUpdate(float fixedDeltaTime)
        {
            // noop
        }

        public virtual void OnExit()
        {
            // noop
        }

        public virtual void HandleInputs(InputReader inputs)
        {
            // noop
        }

        // public virtual void HandleInputs(ref AIInputs inputs)
        // {
        //     // noop
        // }

        public virtual void BeforeCharacterUpdate(float deltaTime)
        {
            // noop
        }

        public virtual void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            // noop
        }

        public virtual void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            // noop
        }

        public virtual void AfterCharacterUpdate(float deltaTime)
        {
            // noop
        }

        public virtual void PostGroundingUpdate(float deltaTime)
        {
            // noop
        }

        public virtual void AddVelocity(Vector3 velocity)
        {
            // noop
        }
    }
}