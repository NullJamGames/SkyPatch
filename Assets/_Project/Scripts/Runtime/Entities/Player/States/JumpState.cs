using UnityEngine;
using CharacterController = NJG.Runtime.Entities.CharacterController;

namespace NJG.Runtime.Entity
{
    public class JumpState : BaseState
    {
        public JumpState(CharacterController character, Animator animator) : base(character, animator) { }

        // private bool _hasBoostedHorizontalSpeed;
        // public override void OnEnter()
        // {
        //     _animator.CrossFade(_jumpHash, _crossFadeDuration);
        //     _hasBoostedHorizontalSpeed = false;
        // }
        //
        // public override void FixedUpdate()
        // {
        //     if (!_hasBoostedHorizontalSpeed)
        //     {
        //         _hasBoostedHorizontalSpeed = true;
        //         _character.JumpBoostHorizontalSpeed();
        //     }
        //     _character.HandleJump();
        //     _character.HandleMovement();
        // }
    }
}