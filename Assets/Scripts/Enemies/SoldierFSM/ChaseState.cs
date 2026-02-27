using UnityEngine;

public class ChaseState : EnemyState
{
    // Burst settings (serialized on EnemyController3D, read via Enemy) ──────
    // shootingTime  — how long the burst lasts
    // shootingTimeVariance — +/- percentage randomised on Enter (0.15 = 15%)
    // reloadTime    — how long the reload pause lasts
    // fireRate      — delay between individual shots during a burst

    private float _fireCooldown;
    private float _outOfRangeTimer;
    private bool  _playerInTrigger;

    // Burst state
    private float _burstTimer;
    private float _randomisedBurstDuration;
    private bool  _reloading;
    private float _reloadTimer;

    public ChaseState(EnemyController3D enemy) : base(enemy) { }

    public override void Enter()
    {
        Enemy.SetRunAnimation(true);

        _fireCooldown   = 0f;
        _outOfRangeTimer = 0f;
        _playerInTrigger = true;

        // Randomise burst duration once on enter so each chase feels different
        RandomiseBurstDuration();

        _burstTimer  = 0f;
        _reloadTimer = 0f;
        _reloading   = false;
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

        // Burst shooting logic
        if (Enemy.IsPlayerInShootRange())
            TickBurst();
    }

    private void TickBurst()
    {
        if (_reloading)
        {
            _reloadTimer += Time.deltaTime;
            if (_reloadTimer >= Enemy.ReloadTime)
            {
                // Reload done — start a fresh burst with a new random duration
                _reloading  = false;
                _reloadTimer = 0f;
                _burstTimer  = 0f;
                _fireCooldown = 0f;
                RandomiseBurstDuration();
            }
            return;
        }

        // Currently in a burst
        _burstTimer += Time.deltaTime;

        _fireCooldown -= Time.deltaTime;
        if (_fireCooldown <= 0f)
        {
            Enemy.Shoot();
            _fireCooldown = Enemy.FireRate;
        }

        if (_burstTimer >= _randomisedBurstDuration)
        {
            // Burst over — start reloading
            _reloading   = true;
            _reloadTimer = 0f;
        }
    }

    private void RandomiseBurstDuration()
    {
        float variance = Enemy.ShootingTime * Enemy.ShootingTimeVariance;
        _randomisedBurstDuration = Random.Range(
            Enemy.ShootingTime - variance,
            Enemy.ShootingTime + variance
        );
    }

    public override void OnPlayerLeftTrigger()
    {
        _playerInTrigger = false;
        _outOfRangeTimer  = 0f;
    }

    public override void Exit()
    {
        _outOfRangeTimer  = 0f;
        _fireCooldown     = 0f;
        _playerInTrigger  = false;
        _reloading        = false;
        _reloadTimer      = 0f;
        _burstTimer       = 0f;
    }
}