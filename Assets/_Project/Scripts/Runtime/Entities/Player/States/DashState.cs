using UnityEngine;
using CharacterController = NJG.Runtime.Entities.CharacterController;

namespace NJG.Runtime.Entity
{
    public class DashState : BaseState
    {
        public DashState(CharacterController character, Animator animator) : base(character, animator) { }

        // public override void OnEnter()
        // {
        //     _animator.CrossFade(_dashHash, _crossFadeDuration);
        // }
        //
        // public override void FixedUpdate()
        // {
        //     _character.HandleMovement();
        // }
    }
}