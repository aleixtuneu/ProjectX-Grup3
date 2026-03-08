using UnityEngine;
using UnityEngine.Rendering;

public class PlayerHealthBehaviour : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private int _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        GameManager.OnPlayerRespawn += HandleRespawn;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerRespawn -= HandleRespawn;
    }

    public void TakeDamage(int damage)
    {
        // GameManager handles respawn invulnerability window
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.GetGameState() != GameState.Playing) return;

        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0);

        Debug.Log($"Player received {damage} damage. Health: {_currentHealth}/{maxHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        GameManager.Instance.PlayerDied();
    }

    // Called by GameManager.OnPlayerRespawn — restore health on respawn
    private void HandleRespawn(Vector3 position, int livesRemaining)
    {
        _currentHealth = maxHealth;
        Debug.Log($"Player health restored to {maxHealth}");
    }
}
