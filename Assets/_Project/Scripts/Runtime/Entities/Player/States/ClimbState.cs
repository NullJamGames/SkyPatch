using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class ClimbState : BaseState
    {
        public ClimbState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            _animator.CrossFade(_climbHash, _crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            _player.HandleClimb();
        }
    }
}