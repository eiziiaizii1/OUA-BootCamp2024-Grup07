﻿//using Codice.Client.Common.GameUI;
//using log4net.Util;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.Animations;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    public enum CharacterType
    {
        Human,
        Beaver,
        Turtle,
        Monkey,
        Kangaroo
    }

    [System.Serializable]
    public struct CharacterProperties
    {
        public float MoveSpeed;
        public float SprintSpeed;
        public float JumpHeight;
        public Vector3 Scale;

        public CharacterProperties(float moveSpeed, float sprintSpeed, float jumpHeight, float scaleFactor)
        {
            MoveSpeed = moveSpeed;
            SprintSpeed = sprintSpeed;
            JumpHeight = jumpHeight;
            Scale = Vector3.one * scaleFactor;
        }
    }

    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif

    public class ThirdPersonController : MonoBehaviour
    {
        private float originalHeight;
        private Vector3 originalCenter;
        private float originalRadius;

        private float originalGroundedRadius;
        private float originalGroundedOffset;

        
        private float originalSpeedChangeRate;
        private float originalFootstepAudioVolume;
        private float originalJumpTimeout;
        private float originalFallTimeout;

        private float originalSlopeLimit;
        private float originalStepOffset;
        private float originalSkinWidth;



        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;


        //public float MoveSpeedRat = 1.0f;
        //public float SprintSpeedRat = 2.335f;
        //public float JumpHeightRat = 0.2f;

        public CharacterType _currentCharacterType = CharacterType.Human;


        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;


        //CLIMBING
        private bool isClimbing = false;
        private Vector3 lastGrabDirection; 

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            //_controller = GetComponent<CharacterController>();
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            originalHeight = _controller.height;
            originalCenter = _controller.center;
            originalRadius = _controller.radius;

            originalGroundedRadius = GroundedRadius;
            originalGroundedOffset = GroundedOffset;

            originalSpeedChangeRate = SpeedChangeRate;
            originalFootstepAudioVolume = FootstepAudioVolume;
            originalJumpTimeout = JumpTimeout;
            originalFallTimeout = FallTimeout;

            originalSlopeLimit = _controller.slopeLimit;
            originalStepOffset = _controller.stepOffset;
            originalSkinWidth = _controller.skinWidth;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // Set target speed based on move speed, sprint speed, and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // Simplistic acceleration and deceleration
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero && !isClimbing)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            if (_currentCharacterType == CharacterType.Monkey)
            {
                if (!isClimbing)
                {
                    float avoidFloorDistance = 0.1f;
                    float climbableGrabDistance = 0.4f;
                    if (Physics.Raycast(transform.position + Vector3.up * avoidFloorDistance, targetDirection, out RaycastHit raycastHit, climbableGrabDistance))
                    {
                        if (raycastHit.transform.CompareTag("Climbable"))
                        {
                            GrabLadder(targetDirection);
                        }
                    }
                }
                else
                {
                    float avoidFloorDistance = 0.1f;
                    float climbableGrabDistance = 0.4f;
                    if (Physics.Raycast(transform.position + Vector3.up * avoidFloorDistance, lastGrabDirection, out RaycastHit raycastHit, climbableGrabDistance))
                    {
                        if (!raycastHit.transform.CompareTag("Climbable"))
                        {
                            DropLadder();
                            _verticalVelocity = 4f;
                        }
                    }
                    else
                    {
                        DropLadder();
                        _verticalVelocity = 4f;
                    }

                    if (Vector3.Dot(targetDirection, lastGrabDirection) < 0f)
                    {
                        float climbableFloorDropDistance = 0.1f;
                        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit floorRaycastHit, climbableFloorDropDistance))
                        {
                            DropLadder();
                        }
                    }

                    // Check if the player is moving downwards
                    if (_input.move.y < 0f)
                    {
                        float groundCheckDistance = 0.2f;
                        // Check if the bottom of the climbable object is ground
                        if (Physics.Raycast(transform.position + Vector3.up * avoidFloorDistance, Vector3.down, out RaycastHit groundHit, groundCheckDistance))
                        {
                            if (groundHit.transform.CompareTag("Ground"))
                            {
                                DropLadder();
                            }
                        }
                    }

                    if (isClimbing)
                    {
                        targetDirection.x = 0f;
                        targetDirection.y = _input.move.y;  // Allow vertical movement (when not rotated)
                        targetDirection.z = 0f;
                        _verticalVelocity = 0f;
                        Grounded = true;
                        _speed = targetSpeed;

                        // Pause or resume climbing animation based on vertical movement
                        bool isMovingVertically = Mathf.Abs(_input.move.y) > 0.1f;
                        _animator.SetFloat("ClimbSpeed", isMovingVertically ? 1.0f : 0.0f);
                    }
                    else
                    {
                        _animator.SetFloat("ClimbSpeed", 0.0f);
                    }
                }
            }

            // Move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // Update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }



        private void GrabLadder(Vector3 lastGrabDirection)
        {
            isClimbing = true;
            _animator.SetBool("isClimbing", true);
            this.lastGrabDirection = lastGrabDirection;
        }

        private void DropLadder()
        {
            _animator.SetBool("isClimbing", false);
            isClimbing = false;
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f && !isClimbing)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        public void SetCharacterProperties(CharacterProperties properties)
        {
            AdjustCharacterController(properties.Scale);

            MoveSpeed = properties.MoveSpeed;
            SprintSpeed = properties.SprintSpeed;
            JumpHeight = properties.JumpHeight;

            transform.GetChild(0).localScale = properties.Scale;
        }

        private void AdjustCharacterController(Vector3 scale)
        {
            _controller.height = originalHeight * scale.y;
            _controller.center = originalCenter * scale.y;
            _controller.radius = originalRadius * scale.x;

            GroundedRadius = originalGroundedRadius * scale.x;
            GroundedOffset = originalGroundedOffset * scale.x;


            SpeedChangeRate = originalSpeedChangeRate * scale.x;
            FootstepAudioVolume = originalFootstepAudioVolume * scale.x;
            JumpTimeout = originalJumpTimeout * scale.z;
            FallTimeout = originalFallTimeout * scale.z;

            //_controller.slopeLimit = originalSlopeLimit * scale.z;
            //_controller.stepOffset = originalStepOffset * scale.z;
            _controller.skinWidth = originalSkinWidth * scale.x;
        }
    }
}