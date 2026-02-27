public abstract class EnemyState
{
    protected EnemyController3D Enemy;

    public EnemyState(EnemyController3D enemy)
    {
        Enemy = enemy;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }

    // Called when the player exits the detection trigger.
    // States opt in by overriding — most can safely ignore it.
    public virtual void OnPlayerLeftTrigger() { }
}
