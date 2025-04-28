using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class InteractState : BaseState
    {
        public InteractState(PlayerController player, Animator animator) : base(player, animator) { }
        
        public override void OnEnter()
        {
            _animator.CrossFade(_attackHash, _crossFadeDuration);
            //_player.Interactor.Interact(_player.Inventory);
        }
        
        public override void FixedUpdate()
        {
            _player.HandleMovement();
        }
    }
}