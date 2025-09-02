using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif

    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Weapon System")]
        private EquipmentSystem _equipmentSystem;
        private WeaponAnimatorController _weaponAnimatorController;

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

        [Header("Slope Handling")]
        [Tooltip("Maximum slope angle the character can walk on")]
        [Range(0f, 90f)]
        public float MaxSlopeAngle = 45f;

        [Tooltip("Force applied when sliding down steep slopes")]
        public float SlideForce = 8f;

        [Tooltip("How much the character should stick to slopes")]
        public float SlopeStickForce = 5f;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player

        public bool canRun => MechanicStateManager.Instance.CanRun;
        public bool canJump => MechanicStateManager.Instance.CanJump;


        public void EnableSprint(bool enable) { MechanicStateManager.Instance.CanRun = enable; }
        public void EnableJump(bool enable) { MechanicStateManager.Instance.CanJump = enable; }

        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // slope handling
        private Vector3 _groundNormal = Vector3.up;
        private bool _isOnSlope;
        private float _slopeAngle;
        private Vector3 _slopeDirection;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private bool _isInitialized = false;
        private int _animIDMove;

#if ENABLE_INPUT_SYSTEM 
        public PlayerInput _playerInput;
#endif
        public Animator _animator;
        private CharacterController _controller;
        public StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;
        private bool _hasAnimator;

        // Minimum walkable slope angle (prevents sticking to walls)
        private const float minWalkableSlope = 0.1f;

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

            _weaponAnimatorController = GetComponent<WeaponAnimatorController>();
        }

        private void Start()
        {
            _equipmentSystem = GetComponent<EquipmentSystem>();
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            // Mark the system as initialized
            _isInitialized = true;
        }

        private void Update()
        {
            HandleWeaponInput();
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

            // Check if we're touching ground
            bool sphereCheck = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // If we detect ground, do a more detailed raycast to get surface normal
            if (sphereCheck)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, GroundedRadius + GroundedOffset + 0.1f, GroundLayers))
                {
                    _groundNormal = hit.normal;
                    _slopeAngle = Vector3.Angle(_groundNormal, Vector3.up);
                    _isOnSlope = _slopeAngle >= minWalkableSlope && _slopeAngle < 90f;

                    // Only consider grounded if slope is walkable OR if we're moving up the slope
                    if (_slopeAngle <= MaxSlopeAngle)
                    {
                        Grounded = true;
                    }
                    else
                    {
                        // On steep slope - not considered grounded for jumping, but handle sliding
                        Grounded = false;
                        _slopeDirection = Vector3.ProjectOnPlane(Vector3.down, _groundNormal).normalized;
                    }
                }
                else
                {
                    // Fallback to sphere check result
                    _groundNormal = Vector3.up;
                    _isOnSlope = false;
                    _slopeAngle = 0f;
                    Grounded = sphereCheck;
                }
            }
            else
            {
                _groundNormal = Vector3.up;
                _isOnSlope = false;
                _slopeAngle = 0f;
                Grounded = false;
            }

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
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = (_input.sprint && canRun) ? SprintSpeed : MoveSpeed;

            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // Calculate movement direction
            Vector3 targetDirection = Vector3.zero;

            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                // Project movement onto ground normal ONLY if walkable slope (> minWalkableSlope and ≤ MaxSlopeAngle)
                if (_isOnSlope && _slopeAngle > minWalkableSlope && _slopeAngle <= MaxSlopeAngle)
                    targetDirection = Vector3.ProjectOnPlane(targetDirection, _groundNormal).normalized;
            }

            // Handle slope sliding for steep slopes
            Vector3 slopeSlideVelocity = Vector3.zero;
            if (_isOnSlope && _slopeAngle > MaxSlopeAngle)
            {
                // Apply sliding force down the slope
                slopeSlideVelocity = _slopeDirection * SlideForce;

                // Reduce player control on steep slopes
                targetDirection *= 0.3f; // Reduce control to 30%
            }

            // Apply movement
            Vector3 horizontalMovement = (targetDirection.normalized * (_speed * Time.deltaTime)) + (slopeSlideVelocity * Time.deltaTime);
            Vector3 verticalMovement = new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;

            // Stick-to-ground ONLY if walkable slope (> minWalkableSlope and ≤ MaxSlopeAngle)
            if (Grounded && _isOnSlope && _slopeAngle > minWalkableSlope && _slopeAngle <= MaxSlopeAngle)
            {
                verticalMovement += -_groundNormal * SlopeStickForce * Time.deltaTime;
            }

            _controller.Move(horizontalMovement + verticalMovement);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void HandleWeaponInput()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("System not initialized. Ignoring weapon input.");
                return;
            }

            if (_input.weaponToggle)
            {
                if (!_equipmentSystem.IsWeaponDrawn() && CanDrawWeapon())
                {
                    _equipmentSystem.DrawWeapon();
                    _input.weaponToggle = false;
                }
                else if (_equipmentSystem.IsWeaponDrawn() && CanSheathWeapon())
                {
                    _equipmentSystem.SheathWeapon();
                    _input.weaponToggle = false;
                }
            }
        }

        private bool CanDrawWeapon() => Grounded;
        private bool CanSheathWeapon() => true;

        public float GetCurrentSpeed()
        {
            return new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;
        }

        public void EnableMovement()
        {
            _animator.SetBool(_animIDMove, true);
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

                // Only reset vertical velocity on flat/very shallow/walkable ground
                if (_verticalVelocity < 0.0f && (!_isOnSlope || _slopeAngle <= MaxSlopeAngle))
                {
                    _verticalVelocity = -2f;
                }

                // Jump (only allow on walkable slopes)
                if (_input.jump && _jumpTimeoutDelta <= 0.0f && (!_isOnSlope || _slopeAngle <= MaxSlopeAngle) && canJump)
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

            // Enhanced gravity for steep slopes
            float gravityMultiplier = 1f;
            if (_isOnSlope && _slopeAngle > MaxSlopeAngle)
            {
                gravityMultiplier = 1.5f; // Apply stronger gravity on steep slopes
            }

            // apply gravity over time if under terminal velocity
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * gravityMultiplier * Time.deltaTime;
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
            Color transparentYellow = new Color(1.0f, 1.0f, 0.0f, 0.35f);

            if (Grounded)
                Gizmos.color = transparentGreen;
            else if (_isOnSlope && _slopeAngle > MaxSlopeAngle)
                Gizmos.color = transparentYellow; // Yellow for steep slopes
            else
                Gizmos.color = transparentRed;

            // Draw the grounded check sphere
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);

            // Draw slope normal if on slope
            if (_isOnSlope)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, _groundNormal * 2f);

                if (_slopeAngle > MaxSlopeAngle)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(transform.position, _slopeDirection * 2f);
                }
            }
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
    }
}