using UnityEngine;
using CharacterController = NJG.Runtime.Entities.CharacterController;

namespace NJG.Runtime.Entity
{
    public class LaunchedState : BaseState
    {
        public LaunchedState(CharacterController character, Animator animator) : base(character, animator)
        {
        }

        // public override void FixedUpdate()
        // {
        //     base.FixedUpdate();
        //     if (_character.Rigidbody.linearVelocity.y < 0) 
        //         _character.HasLaunched = false;
        // }
        //
        // public override void OnExit()
        // {
        //     base.OnExit();
        //     _character.HasLaunched = false;
        // }
    }
}