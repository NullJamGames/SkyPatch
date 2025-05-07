using NJG.Runtime.Entities;
using NJG.Runtime.Input;
using UnityEngine;
using CharacterController = NJG.Runtime.Entities.CharacterController;

namespace NJG.Runtime.Entity
{
    public class LocomotionState : BaseState
    {
        protected Collider[] _probedColliders = new Collider[8];
        protected RaycastHit[] _probedHits = new RaycastHit[8];
        protected Vector3 _moveInputVector;
        protected Vector3 _lookInputVector;
        protected Vector3 _internalVelocityAdd = Vector3.zero;
        
        protected bool _jumpRequested = false;
        protected bool _doubleJumpConsumed = false;
        protected bool _jumpedThisFrame = false;
        protected bool _jumpConsumed = false;
        protected bool _canWallJump = false;
        protected Vector3 _wallJumpNormal;
        protected float _timeSinceJumpRequested = Mathf.Infinity;
        protected float _timeSinceLastAbleToJump = 0f;
        
        protected bool _shouldBeCrouching = false;
        protected bool _isCrouching = false;
        
        public LocomotionState(CharacterController character, Animator animator) : base(character, animator) { }

        // public override void OnEnter()
        // {
        //     _animator.CrossFade(_locomotionHash, _crossFadeDuration);
        // }
        //
        // public override void HandleInputs(InputReader inputs)
        // {
        //     // Clamp inputs
        //     Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveDirection.x, 0f, inputs.MoveDirection.y), 1f);
        //     
        //     Vector3 cameraPlanarDirection =
        //         Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, _character.Motor.CharacterUp).normalized;
        //     if (cameraPlanarDirection.sqrMagnitude == 0f)
        //     {
        //         cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, 
        //             _character.Motor.CharacterUp).normalized;
        //     }
        //     
        //     Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, _character.Motor.CharacterUp);
        //     
        //     // ============== DEFAULT State ==============
        //     // Move and look inputs
        //     _moveInputVector = cameraPlanarRotation * moveInputVector;
        //
        //     switch (_character.OrientationMethod)
        //     {
        //         case OrientationMethod.TowardsCamera:
        //             _lookInputVector = cameraPlanarDirection;
        //             break;
        //         case OrientationMethod.TowardsMovement:
        //             _lookInputVector = _moveInputVector.normalized;
        //             break;
        //     }
        //             
        //     // Jumping input
        //     if (inputs.JumpDown)
        //     {
        //         _timeSinceJumpRequested = 0f;
        //         _jumpRequested = true;
        //     }
        //             
        //     // Crouching input
        //     if (inputs.CrouchDown)
        //     {
        //         _shouldBeCrouching = true;
        //
        //         if (!_isCrouching)
        //         {
        //             _isCrouching = true;
        //             _character.Motor.SetCapsuleDimensions(0.5f, _character.CrouchedCapsuleHeight, 
        //                 _character.CrouchedCapsuleHeight * 0.5f);
        //             // TODO: Probably shouldn't change scale and instead set this in animator...
        //             _character.SetLocalScale(new Vector3(1f, 0.5f, 1f));
        //         }
        //     }
        //     // TODO: If we still need crouching.. this is currently not handled correctly in the input i don't belive...
        //     else if (inputs.CrouchUp)
        //     {
        //         _shouldBeCrouching = false;
        //     }
        // }
        //
        // public override void HandleInputs(ref AIInputs inputs)
        // {
        //     _moveInputVector = inputs.MoveVector;
        //     _lookInputVector = inputs.LookVector;
        // }
        //
        // public override void BeforeCharacterUpdate(float deltaTime) { }
        //
        // public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        // {
        //     if (_lookInputVector.sqrMagnitude > 0f && _character.OrientationSharpness > 0f)
        //     {
        //         // Smoothly interpolate from current to target look direction
        //         Vector3 smoothedLookInputDirection = Vector3.Slerp(_character.Motor.CharacterForward, 
        //             _lookInputVector, 1 - Mathf.Exp(-_character.OrientationSharpness * deltaTime)).normalized;
        //
        //         // Set the current rotation (which will be used by the KinematicCharacterMotor)
        //         currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, _character.Motor.CharacterUp);
        //     }
        //
        //     Vector3 currentUp = currentRotation * Vector3.up;
        //     if (_character.BonusOrientationMethod == BonusOrientationMethod.TowardsGravity)
        //     {
        //         // Rotate from current up to invert gravity
        //         Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -_character.Gravity.normalized, 
        //             1 - Mathf.Exp(-_character.BonusOrientationSharpness * deltaTime));
        //         currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
        //     }
        //     else if (_character.BonusOrientationMethod == BonusOrientationMethod.TowardsGroundSlopeAndGravity)
        //     {
        //         if (_character.Motor.GroundingStatus.IsStableOnGround)
        //         {
        //             Vector3 initialCharacterBottomHemiCenter = _character.Motor.TransientPosition + (currentUp * _character.Motor.Capsule.radius);
        //
        //             Vector3 smoothedGroundNormal = Vector3.Slerp(_character.Motor.CharacterUp, _character.Motor.GroundingStatus.GroundNormal, 
        //                 1 - Mathf.Exp(-_character.BonusOrientationSharpness * deltaTime));
        //             currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;
        //
        //             // Move the position to create a rotation around the bottom hemi center instead of around the pivot
        //             _character.Motor.SetTransientPosition(initialCharacterBottomHemiCenter + 
        //                                                   (currentRotation * Vector3.down * _character.Motor.Capsule.radius));
        //         }
        //         else
        //         {
        //             Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -_character.Gravity.normalized, 
        //                 1 - Mathf.Exp(-_character.BonusOrientationSharpness * deltaTime));
        //             currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
        //         }
        //     }
        //     else
        //     {
        //         Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 
        //             1 - Mathf.Exp(-_character.BonusOrientationSharpness * deltaTime));
        //         currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
        //     }
        // }
        //
        // public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        // {
        //     // Ground movement
        //     if (_character.Motor.GroundingStatus.IsStableOnGround)
        //     {
        //         float currentVelocityMagnitude = currentVelocity.magnitude;
        //
        //         Vector3 effectiveGroundNormal = _character.Motor.GroundingStatus.GroundNormal;
        //
        //         // Reorient velocity on slope
        //         currentVelocity = _character.Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;
        //
        //         // Calculate target velocity
        //         Vector3 inputRight = Vector3.Cross(_moveInputVector, _character.Motor.CharacterUp);
        //         Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
        //         Vector3 targetMovementVelocity = reorientedInput * _character.MaxStableMoveSpeed;
        //
        //         // Smooth movement Velocity
        //         currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 
        //             1f - Mathf.Exp(-_character.StableMovementSharpness * deltaTime));
        //     }
        //     // Air movement
        //     else
        //     {
        //         // Add move input
        //         if (_moveInputVector.sqrMagnitude > 0f)
        //         {
        //             Vector3 addedVelocity = _moveInputVector * (_character.AirAccelerationSpeed * deltaTime);
        //
        //             Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, _character.Motor.CharacterUp);
        //
        //             // Limit air velocity from inputs
        //             if (currentVelocityOnInputsPlane.magnitude < _character.MaxAirMoveSpeed)
        //             {
        //                 // clamp addedVel to make total vel not exceed max vel on inputs plane
        //                 Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, _character.MaxAirMoveSpeed);
        //                 addedVelocity = newTotal - currentVelocityOnInputsPlane;
        //             }
        //             else
        //             {
        //                 // Make sure added vel doesn't go in the direction of the already-exceeding velocity
        //                 if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
        //                 {
        //                     addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
        //                 }
        //             }
        //
        //             // Prevent air-climbing sloped walls
        //             if (_character.Motor.GroundingStatus.FoundAnyGround)
        //             {
        //                 if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
        //                 {
        //                     Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(_character.Motor.CharacterUp, 
        //                         _character.Motor.GroundingStatus.GroundNormal), _character.Motor.CharacterUp).normalized;
        //                     addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
        //                 }
        //             }
        //
        //             // Apply added velocity
        //             currentVelocity += addedVelocity;
        //         }
        //
        //         // Gravity
        //         currentVelocity += _character.Gravity * deltaTime;
        //
        //         // Drag
        //         currentVelocity *= (1f / (1f + (_character.Drag * deltaTime)));
        //     }
        //
        //     // Handle jumping
        //     _jumpedThisFrame = false;
        //     _timeSinceJumpRequested += deltaTime;
        //     if (_jumpRequested)
        //     {
        //         // See if we actually are allowed to jump
        //         if (!_jumpConsumed && ((_character.AllowJumpingWhenSliding ? 
        //                 _character.Motor.GroundingStatus.FoundAnyGround : _character.Motor.GroundingStatus.IsStableOnGround) 
        //                                || _timeSinceLastAbleToJump <= _character.JumpPostGroundingGraceTime))
        //         {
        //             // Calculate jump direction before ungrounding
        //             Vector3 jumpDirection = _character.Motor.CharacterUp;
        //             if (_character.Motor.GroundingStatus.FoundAnyGround && !_character.Motor.GroundingStatus.IsStableOnGround)
        //             {
        //                 jumpDirection = _character.Motor.GroundingStatus.GroundNormal;
        //             }
        //
        //             // Makes the character skip ground probing/snapping on its next update. 
        //             // If this line weren't here, the character would remain snapped to the ground when trying to jump.
        //             _character.Motor.ForceUnground();
        //
        //             // Add to the return velocity and reset jump state
        //             currentVelocity += (jumpDirection * _character.JumpUpSpeed) - Vector3.Project(currentVelocity, 
        //                 _character.Motor.CharacterUp);
        //             currentVelocity += (_moveInputVector * _character.JumpScalableForwardSpeed);
        //             _jumpRequested = false;
        //             _jumpConsumed = true;
        //             _jumpedThisFrame = true;
        //         }
        //     }
        //
        //     // Take into account additive velocity
        //     if (_internalVelocityAdd.sqrMagnitude > 0f)
        //     {
        //         currentVelocity += _internalVelocityAdd;
        //         _internalVelocityAdd = Vector3.zero;
        //     }
        // }
        //
        // public override void AfterCharacterUpdate(float deltaTime)
        // {
        //     // Handle jump-related values
        //     
        //     // Handle jumping pre-ground grace period
        //     if (_jumpRequested && _timeSinceJumpRequested > _character.JumpPreGroundingGraceTime)
        //     {
        //         _jumpRequested = false;
        //     }
        //
        //     if (_character.AllowJumpingWhenSliding ? _character.Motor.GroundingStatus.FoundAnyGround : _character.Motor.GroundingStatus.IsStableOnGround)
        //     {
        //         // If we're on a ground surface, reset jumping values
        //         if (!_jumpedThisFrame)
        //         {
        //             _jumpConsumed = false;
        //         }
        //         _timeSinceLastAbleToJump = 0f;
        //     }
        //     else
        //     {
        //         // Keep track of time since we were last able to jump (for grace period)
        //         _timeSinceLastAbleToJump += deltaTime;
        //     }
        //     
        //     
        //
        //     // Handle uncrouching
        //     if (_isCrouching && !_shouldBeCrouching)
        //     {
        //         // Do an overlap test with the character's standing height to see if there are any obstructions
        //         _character.Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
        //         if (_character.Motor.CharacterOverlap(
        //                 _character.Motor.TransientPosition,
        //                 _character.Motor.TransientRotation,
        //                 _probedColliders,
        //                 _character.Motor.CollidableLayers,
        //                 QueryTriggerInteraction.Ignore) > 0)
        //         {
        //             // If obstructions, just stick to crouching dimensions
        //             _character.Motor.SetCapsuleDimensions(0.5f, _character.CrouchedCapsuleHeight, 
        //                 _character.CrouchedCapsuleHeight * 0.5f);
        //         }
        //         else
        //         {
        //             // If no obstructions, uncrouch
        //             // TODO: Can handle this in animator instead of scale...
        //             _character.MeshRoot.localScale = new Vector3(1f, 1f, 1f);
        //             _isCrouching = false;
        //         }
        //     }
        // }
        //
        // public override void PostGroundingUpdate(float deltaTime)
        // {
        //     // Handle landing and leaving ground
        //     if (_character.Motor.GroundingStatus.IsStableOnGround && !_character.Motor.LastGroundingStatus.IsStableOnGround)
        //     {
        //         _character.OnLanded();
        //     }
        //     else if (!_character.Motor.GroundingStatus.IsStableOnGround && _character.Motor.LastGroundingStatus.IsStableOnGround)
        //     {
        //         _character.OnLeaveStableGround();
        //     }
        // }
        //
        // public override void AddVelocity(Vector3 velocity)
        // {
        //     _internalVelocityAdd += velocity;
        // }

        // public override void FixedUpdate()
        // {
        //     _characterController.HandleMovement();
        // }
    }
}