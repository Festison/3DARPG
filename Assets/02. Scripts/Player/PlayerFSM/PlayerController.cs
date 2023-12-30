using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace Festison
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class PlayerController : MonoBehaviour
    {
        [Header("플레이어 스텟")]
        [Tooltip("캐릭터 이동 속도")]
        public float MoveSpeed = 2.0f;

        [Tooltip("플레이어 달리기 속도")]
        public float SprintSpeed = 5.335f;

        [Tooltip("점프")]
        public float JumpHeight = 1.2f;

        [Tooltip("플레이어에게 가해지는 중력")]
        public float Gravity = -15.0f;

        [Header("자연스러운 애니메이션")]
        private float speed;
        private float animationBlend;
        private float targetRotation = 0.0f;
        private float rotationVelocity;
        private float verticalVelocityDamp;
        private float terminalVelocity = 53.0f;
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10.0f;       
        public float JumpTimeout = 0.50f;
        public float FallTimeout = 0.15f;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        private float jumpTimeoutDelta;
        private float fallTimeoutDelta;

        [Header("플레이어 상태")]
        public bool Grounded = true;
      
        public LayerMask GroundLayers;

        [Header("시네머신")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float CameraAngleOverride = 0.0f;
        public bool LockCameraPosition = false;
        private float cinemachineTargetYaw;
        private float cinemachineTargetPitch;             

        [Header("애니메이션 이름 인트형으로 변환")]
        private int animIDSpeed;
        private int animIDGrounded;
        private int animIDJump;
        private int animIDFreeFall;
        private int animIDMotionSpeed;
        private int animIDRoll;

#if ENABLE_INPUT_SYSTEM 
        public PlayerInput playerInput;
#endif
        [HideInInspector] public Animator animator;
        [HideInInspector] public CharacterController controller;
        [HideInInspector] public PlayerInputsystem inputsystem;
        [HideInInspector] public GameObject mainCamera;

        public StateMachine playerStateMachine;
        public DefaultState defaultState;
        public JumpState jumpState;

        private const float thersold = 0.01f;

        private bool Animating;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            jumpTimeoutDelta = JumpTimeout;
            fallTimeoutDelta = FallTimeout;
            AssignAnimationIDs();
            Animating = TryGetComponent(out animator);
            controller = GetComponent<CharacterController>();
            inputsystem = GetComponent<PlayerInputsystem>();
#if ENABLE_INPUT_SYSTEM 
            playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif          

            playerStateMachine = new StateMachine();
            defaultState = new DefaultState(this, playerStateMachine);
            jumpState = new JumpState(this, playerStateMachine);
            playerStateMachine.Initialize(defaultState);                  
        }

        private void Update()
        {
            GroundedCheck();
            Animating = TryGetComponent(out animator);

            playerStateMachine.currentState.Update();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            animIDSpeed = Animator.StringToHash("Speed");
            animIDGrounded = Animator.StringToHash("Grounded");
            animIDJump = Animator.StringToHash("Jump");
            animIDFreeFall = Animator.StringToHash("FreeFall");
            animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            animIDRoll = Animator.StringToHash("Rolling");
        }

        public void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            if (Animating)
            {
                animator.SetBool(animIDGrounded, Grounded);
            }
        }

        public void Move()
        {
            // 스프린트 상태가 참일시 SprintSpeed 거짓일지 MoveSpeed의 스피드를 대입
            float targetSpeed = inputsystem.sprint ? SprintSpeed : MoveSpeed;

            if (inputsystem.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = inputsystem.analogMovement ? inputsystem.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            else
            {
                speed = targetSpeed;
            }

            animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (animationBlend < 0.01f) animationBlend = 0f;

            Vector3 inputDirection = new Vector3(inputsystem.move.x, 0.0f, inputsystem.move.y).normalized;

            if (inputsystem.move != Vector2.zero)
            {
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                    RotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

            controller.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                             new Vector3(0.0f, verticalVelocityDamp, 0.0f) * Time.deltaTime);

            if (Animating)
            {
                animator.SetFloat(animIDSpeed, animationBlend);
                animator.SetFloat(animIDMotionSpeed, inputMagnitude);
            }
        }

        public void JumpAndGravity()
        {
            if (Grounded)
            {
                fallTimeoutDelta = FallTimeout;

                if (Animating)
                {
                    animator.SetBool(animIDJump, false);
                    animator.SetBool(animIDFreeFall, false);
                }


                if (verticalVelocityDamp < 0.0f)
                {
                    verticalVelocityDamp = -2f;
                }

                // Jump
                if (inputsystem.jump && jumpTimeoutDelta <= 0.0f)
                {
                    verticalVelocityDamp = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    if (Animating)
                    {
                        animator.SetBool(animIDJump, true);
                    }
                }

                if (jumpTimeoutDelta >= 0.0f)
                {
                    jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                jumpTimeoutDelta = JumpTimeout;

                if (fallTimeoutDelta >= 0.0f)
                {
                    fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (Animating)
                    {
                        animator.SetBool(animIDFreeFall, true);
                    }
                }

                inputsystem.jump = false;
            }

            if (verticalVelocityDamp < terminalVelocity)
                verticalVelocityDamp += Gravity * Time.deltaTime;
        }

        public void Roll()
        {
            if (inputsystem.roll)
            {
                if (Animating)
                {
                    animator.SetTrigger(animIDRoll);
                }

                inputsystem.roll = false;
            }            
        }

        private void CameraRotation()
        {
            if (inputsystem.look.sqrMagnitude >= thersold && !LockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                cinemachineTargetYaw += inputsystem.look.x * deltaTimeMultiplier;
                cinemachineTargetPitch += inputsystem.look.y * deltaTimeMultiplier;
            }

            cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride,
                cinemachineTargetYaw, 0.0f);
        }

        // 앵글 최소 최대 제한
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}