using UnityEngine;
using CharacterController = NJG.Runtime.Entities.CharacterController;

namespace NJG.Runtime.Entity
{
    public class InteractState : BaseState
    {
        public InteractState(CharacterController character, Animator animator) : base(character, animator) { }
        
        // public override void OnEnter()
        // {
        //     _animator.CrossFade(_attackHash, _crossFadeDuration);
        //     //_player.Interactor.Interact(_player.Inventory);
        // }
        //
        // public override void FixedUpdate()
        // {
        //     _character.HandleMovement();
        // }
    }
}