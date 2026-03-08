using UnityEngine;

public class TankInteraction : MonoBehaviour
{
    [SerializeField] private float interactionRange = 15f;
    [SerializeField] private TankController tankController;
    [SerializeField] private Transform tankPosition;
    [SerializeField] private Transform playerTransform;

    private bool _isPlayerNear = false;
    private InputSystem_Actions _inputActions;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Enable();
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (!tankPosition || !playerTransform) return;

        float distance = Vector3.Distance(tankPosition.position, playerTransform.position);
        _isPlayerNear = distance < interactionRange;

        // Debug.Log($"Tank pos: {tankPosition.position}, Player pos: {playerTransform.position}");
        // Debug.Log($"Distance: {distance:F2}, Close: {_isPlayerNear}");

        if (_isPlayerNear && _inputActions.Player.Interact.WasPressedThisFrame())
        {
            Debug.Log("E pressed, mounting tank...");
            tankController.Mount(playerTransform);
        }
    }

    private void OnDrawGizmos()
    {
        if (tankPosition != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(tankPosition.position, interactionRange);
        }
    }

    private void OnDestroy()
    {
        if (_inputActions != null)
        {
            _inputActions.Player.Disable();
            _inputActions.Dispose();
        }
    }
}