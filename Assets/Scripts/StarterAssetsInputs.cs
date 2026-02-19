using UnityEngine;
using UnityEngine.InputSystem;


    /// <summary>
    /// Single input authority for the player. Implements <see cref="StarterAssetsInp.IPlayerActions"/>
    /// and forwards every action to the appropriate system — movement state is exposed as properties,
    /// attack is delegated directly to <see cref="ShootBehaviour"/>.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class StarterAssetsInputs : MonoBehaviour, StarterAssetsInp.IPlayerActions
    {
        // -----------------------------------------------------------------------------------------
        // Inspector
        // -----------------------------------------------------------------------------------------

        [Header("Movement Settings")]
        [SerializeField] private bool analogMovement;
        public bool AnalogMovement => analogMovement;

        [Header("Mouse Cursor Settings")]
        [SerializeField] private bool cursorLocked = true;
        [SerializeField] private bool cursorInputForLook = true;

        // -----------------------------------------------------------------------------------------
        // Public input state — read by the character controller every frame
        // -----------------------------------------------------------------------------------------

        public Vector2 Move   { get; private set; }
        public Vector2 Look   { get; private set; }
        public bool    Jump   { get; private set; }
        public bool    Sprint { get; private set; }

        // -----------------------------------------------------------------------------------------
        // Internals
        // -----------------------------------------------------------------------------------------

        private StarterAssetsInp _actions;
        private ShootBehaviour   _shootBehaviour;

        // -----------------------------------------------------------------------------------------
        // Unity lifecycle
        // -----------------------------------------------------------------------------------------

        private void Awake()
        {
            _actions = new StarterAssetsInp();
            _actions.Player.AddCallbacks(this);

            _shootBehaviour = GetComponent<ShootBehaviour>();
            if (_shootBehaviour == null)
                Debug.LogWarning($"{nameof(StarterAssetsInputs)}: no {nameof(ShootBehaviour)} found — Attack input will be ignored.", this);
        }

        private void OnEnable()
        {
            _actions.Player.Enable();
            ApplyCursorState(cursorLocked);
        }

        private void OnDisable()
        {
            _actions.Player.Disable();
        }

        private void OnDestroy()
        {
            _actions.Player.RemoveCallbacks(this);
            _actions.Dispose();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            ApplyCursorState(cursorLocked);
        }

        // -----------------------------------------------------------------------------------------
        // StarterAssetsInp.IPlayerActions
        // -----------------------------------------------------------------------------------------

        public void OnMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (!cursorInputForLook) return;
            Look = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            Jump = context.performed;
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            Sprint = context.performed;
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                _shootBehaviour?.TryShoot();
        }

        // -----------------------------------------------------------------------------------------
        // Cursor helpers
        // -----------------------------------------------------------------------------------------

        public void SetCursorLocked(bool locked)
        {
            cursorLocked = locked;
            ApplyCursorState(locked);
        }

        private static void ApplyCursorState(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }