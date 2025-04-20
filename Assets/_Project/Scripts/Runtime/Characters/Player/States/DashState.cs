using UnityEngine;

namespace NJG.Runtime.Characters.Player.States
{
    public class DashState : BaseState
    {
        public DashState(PlayerController player, Animator animator) : base(player, animator) { }
        
        public override void OnEnter()
        {
            _animator.CrossFade(_dashHash, _crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            _player.HandleMovement();
        }
    }
}