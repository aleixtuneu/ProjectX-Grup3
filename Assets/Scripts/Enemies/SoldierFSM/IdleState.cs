public class IdleState : EnemyState
{
    public IdleState(EnemyController3D enemy) : base(enemy) { }

    public override void Enter()
    {
        Enemy.SetRunAnimation(false);
    }

    // No Update logic needed — OnTriggerEnter on the controller
    // handles the transition to ChaseState automatically.
}
