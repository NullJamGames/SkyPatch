using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class LaunchedState : BaseState
    {
        public LaunchedState(PlayerController player, Animator animator) : base(player, animator)
        {
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (_player.Rigidbody.linearVelocity.y < 0) 
                _player.HasLaunched = false;
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.HasLaunched = false;
        }
    }
}