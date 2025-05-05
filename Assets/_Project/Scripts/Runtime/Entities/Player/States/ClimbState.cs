using NJG.Runtime.Entities;
using NJG.Runtime.Interactables;
using UnityEngine;
using CharacterController = NJG.Runtime.Entities.CharacterController;

namespace NJG.Runtime.Entity
{
    public class ClimbState : BaseState
    {
        protected enum ClimbingState { Anchoring, Climbing, DeAnchoring}
        
        protected float _ladderUpDownInput;
        protected Ladder _activeLadder { get; set; }
        protected ClimbingState _internalClimbingState;
        protected ClimbingState _climbingState
        {
            get => _internalClimbingState;
            set
            {
                _internalClimbingState = value;
                _anchoringTimer = 0f;
                _anchoringStartPosition = _character.Motor.TransientPosition;
                _anchoringStartRotation = _character.Motor.TransientRotation;
            }
        }
        protected Vector3 _ladderTargetPosition;
        protected Quaternion _ladderTargetRotation;
        protected float _onLadderSegmentState = 0;
        protected float _anchoringTimer = 0f;
        protected Vector3 _anchoringStartPosition = Vector3.zero;
        protected Quaternion _anchoringStartRotation = Quaternion.identity;
        protected Quaternion _rotationBeforeClimbing = Quaternion.identity;
        
        public ClimbState(CharacterController character, Animator animator) : base(character, animator) { }

        public override void OnEnter()
        {
            _rotationBeforeClimbing = _character.Motor.TransientRotation;

            _character.Motor.SetMovementCollisionsSolvingActivation(false);
            _character.Motor.SetGroundSolvingActivation(false);
            _climbingState = ClimbingState.Anchoring;

            // Store the target position and rotation to snap to
            _ladderTargetPosition = _activeLadder.ClosestPointOnLadderSegment(_character.Motor.TransientPosition, out _onLadderSegmentState);
            _ladderTargetRotation = _activeLadder.transform.rotation;
        }

        // public override void HandleInputs(ref PlayerCharacterInputs inputs)
        // {
        //     _ladderUpDownInput = inputs.MoveAxisForward;
        // }
        //
        // public override void OnExit()
        // {
        //     _character.Motor.SetMovementCollisionsSolvingActivation(true);
        //     _character.Motor.SetGroundSolvingActivation(true);
        // }

        // public override void OnEnter()
        // {
        //     _animator.CrossFade(_climbHash, _crossFadeDuration);
        // }
        //
        // public override void FixedUpdate()
        // {
        //     _character.HandleClimb();
        // }
    }
}