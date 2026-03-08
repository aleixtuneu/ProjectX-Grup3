using UnityEngine;

public class EnemyHealthBehaviour : MonoBehaviour, IDamagable
{
    [SerializeField] private int maxHealth = 30;

    private int _currentHealth;
    private ICreature _creatureComponent;

    private void Awake()
    {
        _currentHealth = maxHealth;
        _creatureComponent = GetComponent<ICreature>();
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        Debug.Log($"{gameObject.name} received {damage} damage. Health: {_currentHealth}/{maxHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_creatureComponent != null)
            _creatureComponent.OnHealthDepleted();
        else
            Destroy(gameObject);
    }
}