using NJG.Utilities.PredicateStateMachines;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class JumpState : BaseState
    {
        public JumpState(PlayerController player, Animator animator) : base(player, animator) { }

        private bool _hasBoostedHorizontalSpeed;
        public override void OnEnter()
        {
            _animator.CrossFade(_jumpHash, _crossFadeDuration);
            _hasBoostedHorizontalSpeed = false;
        }

        public override void FixedUpdate()
        {
            if (!_hasBoostedHorizontalSpeed)
            {
                _hasBoostedHorizontalSpeed = true;
                _player.JumpBoostHorizontalSpeed();
            }
            _player.HandleJump();
            _player.HandleMovement();
        }
    }
}