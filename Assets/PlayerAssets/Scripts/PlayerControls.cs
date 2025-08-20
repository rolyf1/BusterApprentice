using UnityEngine;
using UnityEngine.InputSystem;


namespace PlayerMovement
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif

    public class PlayerMovement : MonoBehaviour
    {
        public float walkSpeed = 2.0f;
        public float runSpeed = 6.0f;
        public float jumpHeight = 1.2f;
        public bool isGrounded = true;
        public float jumpTimeout = 0.50f;
        public float GroundedOffset = -0.14f; // for rough ground
        public float GroundedRadius = 0.28f; // based on radius of character controller component
        public float fallTimeout = 0.15f;
        public float gravity = -15.0f;
        public bool lockCamPos = true;
        public LayerMask GroundLayers;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        // Cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        public GameObject CMCamTarget;
        public float topClamp = 70.0f;
        public float bottomClamp = -30.0f;
        public float camAngleOverride = 0.0f;

        // Player attributes
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // animation IDs
        private int _animIDMove;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        // Code
#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private GameObject _mainCamera;

        public 
    }
}
