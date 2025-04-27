using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class ClimbState : BaseState
    {
        public ClimbState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
        }

        public override void FixedUpdate()
        {
            _player.HandleClimb();
        }

        

    }
}