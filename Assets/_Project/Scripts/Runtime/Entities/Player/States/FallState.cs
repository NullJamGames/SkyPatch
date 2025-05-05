using UnityEngine;
using CharacterController = NJG.Runtime.Entities.CharacterController;

namespace NJG.Runtime.Entity
{
    public class FallState : BaseState
    {
        public FallState(CharacterController character, Animator animator) : base(character, animator) { }

        // public override void OnEnter()
        // {
        //     _animator.CrossFade(_fallHash, _crossFadeDuration);
        // }
        //
        // public override void FixedUpdate()
        // {
        //     base.Update();
        //     
        //     _character.HandleMovement();
        // }
    }
}