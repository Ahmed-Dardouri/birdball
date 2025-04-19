using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class PlayerController : MonoBehaviour, IPlayerController
    {
        public LayerMask PlayerLayer;


        #region stats

        [Tooltip("Set this to the layer your wall is on")]
        public LayerMask wallLayer;

        [Header("INPUT")] [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
        public bool SnapInput = true;

        [Tooltip("Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers"), Range(0.01f, 0.99f)]
        public float VerticalDeadZoneThreshold = 0.3f;

        [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
        public float HorizontalDeadZoneThreshold = 0.1f;

        [Header("MOVEMENT")] [Tooltip("The top horizontal movement speed")]
        public float MaxSpeed = 8;

        [Tooltip("The player's capacity to gain horizontal speed")]
        public float Acceleration = 120;

        [Tooltip("The pace at which the player comes to a stop")]
        public float GroundDeceleration = 150;

        [Tooltip("Deceleration in air only after stopping input mid-air")]
        public float AirDeceleration = 150;

        [Tooltip("A constant downward force applied while grounded. Helps on slopes"), Range(0f, -10f)]
        public float GroundingForce = -1.5f;

        [Tooltip("The detection distance for grounding and roof detection"), Range(0f, 0.5f)]
        public float GrounderDistance = 0.05f;

        [Tooltip("The detection distance for walls detection"), Range(0f, 0.5f)]
        public float WallerDistance = 0.05f;

        [Header("JUMP")] [Tooltip("The immediate vertical velocity applied when jumping")]
        public float JumpPower = 36;

        [Tooltip("The immediate horizontal velocity applied when jumping from wall")]
        public float JumpFromWallPower = 10;

        [Tooltip("The maximum vertical movement speed")]
        public float MaxFallSpeed = 40;

        [Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity")]
        public float FallAcceleration = 110;

        [Tooltip("The gravity multiplier added when jump is released early")]
        public float JumpEndEarlyGravityModifier = 3;

        [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
        public float CoyoteTime = .15f;

        public float JumpBuffer = .2f;
        #endregion

        private Rigidbody2D _rb;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        public LayerMask groundLayer;
        public Transform groundCheckPoint;


        
        #region flap_declaration

        public Transform leftWing;
        public Transform rightWing;

        public float flapSpeed = 10f;

        private MovementTracker _MovementTracker;

        private DashController _DashController;
        private JumpController _JumpController;

        private Quaternion leftClosed;
        private Quaternion leftOpen;
        private Quaternion rightClosed;
        private Quaternion rightOpen;
        
        #endregion

        private bool _applyGravityCheck;

        public bool IsJumping(){
            return !_grounded;
        }

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        #endregion

        private float _time;

        #region Start

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;

            _applyGravityCheck = false;
            
            DashControllerInit();
            JumpControllerInit();
            MovementTrackerInit();

            FlapInit();
        }

        private void DashControllerInit(){
            _DashController.velocity = 30f;
            _DashController.duration = 0.05f;
            _DashController.cooldown = 0.15f;
            _DashController.direction = new Vector2();
            _DashController.isDashing = false;
            _DashController.cooldownPassed = true;
            _DashController.canDash = true;
        }

        private void JumpControllerInit(){
            _JumpController.JumpToConsume = false;
            _JumpController.BufferedJumpUsable = false;
            _JumpController.EndedJumpEarly = false;
            _JumpController.CoyoteUsable = false;
            _JumpController.TimeJumpWasPressed = 0;
            _JumpController.canJump = true;
            _JumpController.jumpEnabled = true;
        }

        private void MovementTrackerInit(){
            _MovementTracker.lastMove = new Vector2(1f, 0);
            _MovementTracker.lastHorizontalMove = 0f;
            _MovementTracker.lastVeticalMove = 0f;
            _MovementTracker.horizontalIsPressed = false;
            _MovementTracker.horizontalIsPressed_prevState = false;
            _MovementTracker.verticalIsPressed = false;
            _MovementTracker.verticalIsPressed_prevState = false;
        }

        private void FlapInit(){
            leftClosed = leftWing.localRotation;
            rightClosed = rightWing.localRotation;

            leftOpen = Quaternion.Euler(0, 0, 0);
            rightOpen = Quaternion.Euler(0, 0, 0);
        }

        #endregion

        private void Update()
        {
            _time += Time.deltaTime;
            GatherInput();
            CheckDashTask();
            MovementTrackerTask();
            ApplyGravityCheck();
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };


            if (SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
                
            }

            if (_frameInput.JumpDown)
            {
                _JumpController.JumpToConsume = true;
                _JumpController.TimeJumpWasPressed = _time;
            }
        }

        private void ApplyGravityCheck(){
            if(_DashController.isDashing == true){
                _applyGravityCheck = false;
            }else{
                _applyGravityCheck = true;
            }
        }

        private void MovementTrackerTask(){
            Vector2 Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            _MovementTracker.horizontalIsPressed    = Move.x != 0;
            _MovementTracker.verticalIsPressed      = Move.y != 0;

            if(_MovementTracker.horizontalIsPressed){
                _MovementTracker.lastHorizontalMove = Move.x;
                _MovementTracker.lastMove = new Vector2(Move.x, 0);
            }

            if(_MovementTracker.verticalIsPressed){
                _MovementTracker.lastVeticalMove = Move.y;
                _MovementTracker.lastMove = new Vector2(0, Move.y);
            }

            _MovementTracker.horizontalIsPressed_prevState = _MovementTracker.horizontalIsPressed;
            _MovementTracker.verticalIsPressed_prevState = _MovementTracker.verticalIsPressed;
        }


        #region Flap
        
        private System.Collections.IEnumerator FlapWings(){
            float t = 0f;

            // Open wings
            while (t < 1f)
            {
                t += Time.deltaTime * flapSpeed;
                leftWing.localRotation = Quaternion.Lerp(leftClosed, leftOpen, t);
                rightWing.localRotation = Quaternion.Lerp(rightClosed, rightOpen, t);
                yield return null;
            }

            t = 0f;

            // Close wings
            while (t < 1f)
            {
                t += Time.deltaTime * flapSpeed;
                leftWing.localRotation = Quaternion.Lerp(leftOpen, leftClosed, t);
                rightWing.localRotation = Quaternion.Lerp(rightOpen, rightClosed, t);
                yield return null;
            }
        }

        private System.Collections.IEnumerator FlapLeftWing(){
            float t = 0f;

            // Open wings
            while (t < 1f)
            {
                t += Time.deltaTime * flapSpeed;
                leftWing.localRotation = Quaternion.Lerp(leftClosed, leftOpen, t);
                yield return null;
            }

            t = 0f;

            // Close wings
            while (t < 1f)
            {
                t += Time.deltaTime * flapSpeed;
                leftWing.localRotation = Quaternion.Lerp(leftOpen, leftClosed, t);
                yield return null;
            }
        }

        private System.Collections.IEnumerator FlapRightWing(){
            float t = 0f;

            // Open wings
            while (t < 1f)
            {
                t += Time.deltaTime * flapSpeed;
                rightWing.localRotation = Quaternion.Lerp(rightClosed, rightOpen, t);
                yield return null;
            }

            t = 0f;

            // Close wings
            while (t < 1f)
            {
                t += Time.deltaTime * flapSpeed;
                rightWing.localRotation = Quaternion.Lerp(rightOpen, rightClosed, t);
                yield return null;
            }
        }
      
        #endregion

        #region Dash

        private void CheckDashTask(){
            if(Input.GetKeyDown("e") && _DashController.canDash && _DashController.cooldownPassed){

                _DashController.isDashing = true;
                _DashController.canDash = false;  
                _DashController.cooldownPassed = false;

                _DashController.direction = new Vector2(_MovementTracker.lastHorizontalMove, 0);

                if(_DashController.direction == Vector2.zero){
                    _DashController.direction = new Vector2(transform.localScale.x, 0);
                }
                // stop dash
                StartCoroutine(StopDash());
                if(_DashController.direction == Vector2.right){
                    StartCoroutine(FlapLeftWing());
                }else{
                    StartCoroutine(FlapRightWing());
                }
            }
            RunDash();
        }

        private IEnumerator StopDash(){
            yield return new WaitForSeconds(_DashController.duration);
            _DashController.isDashing = false;
            yield return new WaitForSeconds(_DashController.cooldown);
            _DashController.cooldownPassed = true;
        }

        private void RunDash(){
            if(_DashController.isDashing){
                _frameVelocity = _DashController.direction.normalized * _DashController.velocity;
            }else if(_grounded){
                _DashController.canDash = true;
            }
        }

        #endregion

        private void FixedUpdate()
        {
            CheckCollisions();

            HandleJump();

            // do not change order
            HandleHorizontalDirection();
            HandleGravity();
            
            ApplyMovement();
        }

        #region Collisions
        
        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void CheckCollisions(){
            Physics2D.queriesStartInColliders = false;

            bool groundHit = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, GrounderDistance, groundLayer);

            // Landed on the Ground

            if (!_grounded && groundHit)
            {
                _grounded = true;
                _JumpController.CoyoteUsable = true;
                _JumpController.BufferedJumpUsable = true;
                _JumpController.EndedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));

            }
            // Left the Ground
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        #endregion

        #region Jumping

        private bool HasBufferedJump => _JumpController.BufferedJumpUsable && _time < _JumpController.TimeJumpWasPressed + JumpBuffer;
        private bool CanUseCoyote => _JumpController.CoyoteUsable && !_grounded && _time < _frameLeftGrounded + CoyoteTime;

        private void HandleJump()
        {
            _JumpController.canJump = false;
            if (!_JumpController.EndedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0){
                _JumpController.EndedJumpEarly = true;
            } 
            if (!_JumpController.JumpToConsume && !HasBufferedJump){
                return;
            }

            if(_grounded || CanUseCoyote){
                _JumpController.canJump = true;
            }

            if(_JumpController.canJump){
                ExecuteJump();
                StartCoroutine(FlapWings());
            }
            
            _JumpController.JumpToConsume = false;
        }

  
        private void ExecuteJump()
        {
            _JumpController.EndedJumpEarly = false;
            _JumpController.TimeJumpWasPressed = 0;
            _JumpController.BufferedJumpUsable = false;
            _JumpController.CoyoteUsable = false;      
            _frameVelocity.y = JumpPower;
            Jumped?.Invoke();
        }

        #endregion

        #region Horizontal

        private void HandleHorizontalDirection()
        {
            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? GroundDeceleration : AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * MaxSpeed, Acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (_grounded && _frameVelocity.y <= 0f){
                _frameVelocity.y = GroundingForce;
            }else if(_applyGravityCheck == true){
                var inAirGravity = FallAcceleration;
                if (_JumpController.EndedJumpEarly && _frameVelocity.y > 0) inAirGravity *= JumpEndEarlyGravityModifier;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        private void ApplyMovement() => _rb.linearVelocity = _frameVelocity;

    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public struct DashController
    {
        public float velocity;
        public float duration;
        public float cooldown;
        public bool cooldownPassed;
        public Vector2 direction;
        public bool isDashing;
        public bool canDash;
    }
    
    public struct JumpController
    {
        public bool JumpToConsume;
        public bool BufferedJumpUsable;
        public bool EndedJumpEarly;
        public bool CoyoteUsable;
        public float TimeJumpWasPressed;
        public bool canJump;
        public bool jumpEnabled;
    }

    public struct MovementTracker
    {
        public Vector2 lastMove;
        public float lastHorizontalMove;
        public float lastVeticalMove;
        public bool horizontalIsPressed_prevState;
        public bool verticalIsPressed_prevState;
        public bool horizontalIsPressed;
        public bool verticalIsPressed;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }

