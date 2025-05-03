using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class FallState : BaseState
    {
        public FallState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            _animator.CrossFade(_fallHash, _crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            base.Update();
            
            _player.HandleMovement();
        }
    }
}