using NJG.Utilities.PredicateStateMachines;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class JumpState : BaseState
    {
        public JumpState(PlayerController player, Animator animator) : base(player, animator) { }
        
        public override void OnEnter()
        {
            _animator.CrossFade(_jumpHash, _crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            _player.HandleJump();
            _player.HandleMovement();
        }
    }
}