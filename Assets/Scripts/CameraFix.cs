using UnityEngine;
using UnityEngine.InputSystem;


public class CameraFix : MonoBehaviour
{
    [Header("Configuració Càmera")]
    [Tooltip("Límit de rotació cap amunt (graus)")]
    public float maxLookUp = 80f;

    [Tooltip("Límit de rotació cap avall (graus)")]
    public float maxLookDown = -80f;

    [Tooltip("Sensibilitat del mouse")]
    public float mouseSensitivity = 2f;

    private float rotationX = 0f;
    private Transform playerBody;
    private PlayerInput playerInput;
    private InputAction lookAction;

    void Start()
    {
        // Bloquejar el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Buscar el cos del jugador
        playerBody = transform.parent;
        if (playerBody == null)
            playerBody = transform;

        // BUSCAR PlayerInput - pot estar al pare o més amunt!
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.Log("?? PlayerInput no trobat aquí, buscant al pare...");
            playerInput = GetComponentInParent<PlayerInput>();
        }

        if (playerInput != null)
        {
            // Buscar l'acció Look
            try
            {
                lookAction = playerInput.actions["Look"];
                Debug.Log($"? CameraFix inicialitzat! Acció 'Look' trobada a {playerInput.gameObject.name}");
            }
            catch
            {
                Debug.LogWarning("?? No s'ha trobat l'acció 'Look'. Provant amb 'Player/Look'...");
                try
                {
                    lookAction = playerInput.actions["Player/Look"];
                    Debug.Log($"? Acció 'Player/Look' trobada!");
                }
                catch
                {
                    Debug.LogError("? No s'ha trobat l'acció 'Look' en el PlayerInput!");
                    Debug.LogError("?? SOLUCIÓ: Assegura't que el teu InputSystem_Actions té l'acció 'Look' configurada.");
                }
            }
        }
        else
        {
            Debug.LogError("? NO S'HA TROBAT PlayerInput! El moviment de càmera no funcionarà.");
            Debug.LogError("?? SOLUCIÓ: Assegura't que el PlayerCapsule (o el GameObject pare) té el component PlayerInput.");
        }
    }

    void Update()
    {
        if (lookAction == null) return;

        // Obtenir input del mouse amb New Input System
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Calcular rotació vertical
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, maxLookDown, maxLookUp);

        // Aplicar rotació local a la càmera (només vertical)
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}