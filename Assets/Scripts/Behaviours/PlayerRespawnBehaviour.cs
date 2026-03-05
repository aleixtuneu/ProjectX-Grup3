using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerRespawnBehaviour : MonoBehaviour
{
    [SerializeField] private float inputDisableDuration = 1f; // seconds without control after respawn

    private CharacterController _characterController;
    private InputSystem_Actions _inputActions;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Enable();
    }

    private void OnEnable()
    {
        GameManager.OnPlayerRespawn += HandleRespawn;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerRespawn -= HandleRespawn;
    }

    private void OnDestroy()
    {
        if (_inputActions != null)
        {
            _inputActions.Player.Disable();
            _inputActions.Dispose();
        }
    }

    private void HandleRespawn(Vector3 respawnPosition, int livesRemaining)
    {
        StartCoroutine(RespawnRoutine(respawnPosition));
    }

    private IEnumerator RespawnRoutine(Vector3 respawnPosition)
    {
        // 1. Disable player input immediately
        _inputActions.Player.Disable();
        Debug.Log("Respawn: input disabled");

        // 2. Teleport player — CharacterController.Move ignores SetPosition,
        //    so we disable it first, move, then re-enable.
        _characterController.enabled = false;
        transform.position = respawnPosition;
        _characterController.enabled = true;
        Debug.Log($"Respawn: player moved to {respawnPosition}");

        // 3. Wait before restoring control
        yield return new WaitForSeconds(inputDisableDuration);

        // 4. Restore input
        _inputActions.Player.Enable();
        Debug.Log("Respawn: input restored");

        // 5. Tell GameManager the player is ready to take damage again
        GameManager.Instance?.HasRespawned();
    }
}
