using UnityEngine;

public class TestCreatureBehaviour : MonoBehaviour, ICreature
{
    private SequentialSpawnerBehaviour _spawner;
    
    [SerializeField] private float lifeTime = 3f; // Dies after 3 seconds for testing

    public void SetSpawner(SequentialSpawnerBehaviour spawner)
    {
        this._spawner = spawner;
    }

    private void Start()
    {
        // Auto-die after lifeTime for testing
        // TODO: remove automatic death, and have the creature call OnHealthDepleted() when dead from external damage instead.
        Invoke(nameof(Die), lifeTime);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} is dying!");
        
        // Notify spawner
        if (_spawner)
        {
            _spawner.OnCreatureDeath();
        }

        Destroy(gameObject);
    }

    // Call this method when your creature actually dies (e.g., health reaches 0)
    public void OnHealthDepleted()
    {
        Die();
    }
}