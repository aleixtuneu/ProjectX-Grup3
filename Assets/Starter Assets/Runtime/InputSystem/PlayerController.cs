using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        // Referencia al sistema de disparo
        private ShootBehaviour _shootBehaviour;

        private void Awake()
        {
            _shootBehaviour = GetComponent<ShootBehaviour>();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        // Estos métodos los llama automáticamente el componente PlayerInput
        public void OnMove(InputValue value)
        {
            move = value.Get<Vector2>();
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
                look = value.Get<Vector2>();
        }

        public void OnJump(InputValue value)
        {
            jump = value.isPressed;
        }

        public void OnSprint(InputValue value)
        {
            sprint = value.isPressed;
        }

        public void OnAttack(InputValue value)
        {
            Debug.Log("OnAttack Called"); //
            if (value.isPressed)
                _shootBehaviour?.TryShoot();
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}