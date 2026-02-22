using UnityEngine;

public class ChaseState : EnemyState
{
    private float _fireCooldown;
    private float _outOfRangeTimer;
    private bool _playerInTrigger;

    public ChaseState(EnemyController3D enemy) : base(enemy) { }

    public override void Enter()
    {
        Enemy.SetRunAnimation(true);
        _fireCooldown = 0f;
        _outOfRangeTimer = 0f;
        _playerInTrigger = true;
    }

    public override void Update()
    {
        if (!_playerInTrigger)
        {
            _outOfRangeTimer += Time.deltaTime;
            if (_outOfRangeTimer >= Enemy.ReturnDelay)
                Enemy.TransitionTo(Enemy.ReturnState);
            return;
        }

        // Always face the player horizontally
        Enemy.RotateTowardsPlayer();

        // Walk closer only if outside shoot range and within wander limit
        if (!Enemy.IsPlayerInShootRange() && !Enemy.HasReachedWanderLimit())
            Enemy.MoveTowardsPlayer();

        // Shoot when close enough
        if (Enemy.IsPlayerInShootRange())
        {
            _fireCooldown -= Time.deltaTime;
            if (_fireCooldown <= 0f)
            {
                Enemy.Shoot();
                _fireCooldown = Enemy.FireRate;
            }
        }
    }

    public override void OnPlayerLeftTrigger()
    {
        _playerInTrigger = false;
        _outOfRangeTimer = 0f;
    }

    public override void Exit()
    {
        _outOfRangeTimer = 0f;
        _fireCooldown = 0f;
        _playerInTrigger = false;
    }
}
