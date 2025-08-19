// Add/replace inside PlayerInputSystem.cs
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerAssets
{
    public class PlayerInputSystem : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public bool jump;
        public bool sprint;
        public bool interact;
        public bool analogMovement;

        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _interactAction;
        private InputAction _sprintAction;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

            // Find actions by name — names must match the ones in your Input Actions asset
            _moveAction = _playerInput.actions.FindAction("Move");
            _interactAction = _playerInput.actions.FindAction("Interact");
            _sprintAction = _playerInput.actions.FindAction("Sprint");

            if (_moveAction == null) Debug.LogWarning("Move action not found in PlayerInput.actions");
            if (_interactAction == null) Debug.LogWarning("Interact action not found in PlayerInput.actions");
            if (_sprintAction == null) Debug.LogWarning("Sprint action not found in PlayerInput.actions");
        }

        private void OnEnable()
        {
            if (_moveAction != null)
            {
                _moveAction.performed += OnMovePerformed;
                _moveAction.canceled += OnMoveCanceled;
                _moveAction.Enable();
            }

            if (_interactAction != null)
            {
                _interactAction.performed += OnInteractPerformed;
                _interactAction.canceled += OnInteractCanceled;
                _interactAction.Enable();
            }

            if (_sprintAction != null)
            {
                _sprintAction.performed += OnSprintPerformed;
                _sprintAction.canceled += OnSprintCanceled;
                _sprintAction.Enable();
            }
        }

        private void OnDisable()
        {
            if (_moveAction != null)
            {
                _moveAction.performed -= OnMovePerformed;
                _moveAction.canceled -= OnMoveCanceled;
                _moveAction.Disable();
            }

            if (_interactAction != null)
            {
                _interactAction.performed += OnInteractPerformed;
                _interactAction.canceled += OnInteractCanceled;
                _interactAction.Disable();
            }

            if (_sprintAction != null)
            {
                _sprintAction.performed += OnSprintPerformed;
                _sprintAction.canceled += OnSprintCanceled;
                _sprintAction.Disable();
            }
        }

        // ---- callbacks ----
        // Move
        private void OnMovePerformed(InputAction.CallbackContext ctx) => MoveInput(ctx.ReadValue<Vector2>());
        private void OnMoveCanceled(InputAction.CallbackContext ctx) => MoveInput(Vector2.zero);

        // Interact
        private void OnInteractPerformed(InputAction.CallbackContext ctx) => InteractInput(true);
        private void OnInteractCanceled(InputAction.CallbackContext ctx) => InteractInput(false);

        // Sprint
        private void OnSprintPerformed(InputAction.CallbackContext ctx) => SprintInput(true);
        private void OnSprintCanceled(InputAction.CallbackContext ctx) => SprintInput(false);

        // ---- existing helpers (keep or adapt your existing methods) ----
        public void MoveInput(Vector2 newMoveDirection) => move = newMoveDirection;
        public void InteractInput(bool pressed) => interact = pressed;
        public void SprintInput(bool pressed) => sprint = pressed;
        public void JumpInput(bool pressed) => jump = pressed;
    }
}
