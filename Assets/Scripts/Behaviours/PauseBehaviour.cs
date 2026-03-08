using UnityEngine;
using UnityEngine.InputSystem;

public class PauseBehaviour : MonoBehaviour
{
    [SerializeField] private Key pauseKey = Key.Escape;

    private void Update()
    {
        if (Keyboard.current[pauseKey].wasPressedThisFrame)
        {
            GameManager.Instance?.TogglePause();
        }
    }
}
