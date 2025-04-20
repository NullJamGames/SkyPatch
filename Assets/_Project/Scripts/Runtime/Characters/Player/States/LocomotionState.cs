using NJG.Runtime.StateMachine;
using UnityEngine;

namespace NJG.Runtime.Characters.Player.States
{
    public class LocomotionState : BaseState
    {
        public LocomotionState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            _animator.CrossFade(_locomotionHash, _crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            _player.HandleMovement();
        }
    }
}