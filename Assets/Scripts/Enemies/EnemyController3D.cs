using UnityEngine;

// Requires a SphereCollider set as Is Trigger on this GameObject (or a child).
// That collider's radius defines the detection range.
// Requires a second, smaller SphereCollider (non-trigger) for physics if needed.
[RequireComponent(typeof(Animator))]
public class EnemyController3D : MonoBehaviour, ICreature
{
    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float shootRange = 6f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float maxWanderDistance = 4f;

    [Header("Behaviour")]
    [SerializeField] private float returnDelay = 5f;

    // ── Public read-only data for states ──────────────────────────────────────
    public float ReturnDelay     => returnDelay;
    public float FireRate        => fireRate;
    public Transform Target      { get; private set; }
    public Vector3 OriginPosition    { get; private set; }
    public Quaternion OriginRotation { get; private set; }

    // ── State instances ───────────────────────────────────────────────────────
    public IdleState           IdleState   { get; private set; }
    public ChaseState          ChaseState  { get; private set; }
    public ReturnToOriginState ReturnState { get; private set; }

    private EnemyState _currentState;
    private Animator _animator;
    private SequentialSpawnerBehaviour _spawner;

    private static readonly int IsRunning = Animator.StringToHash("IsRunning");

    // ── Unity lifecycle ───────────────────────────────────────────────────────

    private void Start()
    {
        _animator = GetComponent<Animator>();

        OriginPosition = transform.position;
        OriginRotation = transform.rotation;

        if (!firePoint)
            firePoint = transform;

        IdleState   = new IdleState(this);
        ChaseState  = new ChaseState(this);
        ReturnState = new ReturnToOriginState(this);

        TransitionTo(IdleState);
    }

    private void Update()
    {
        _currentState?.Update();
    }

    // ── Trigger-based detection ───────────────────────────────────────────────
    // If the SphereCollider (Is Trigger) is on THIS GameObject, Unity calls these directly.
    // If it's on a child, add EnemyDetectionTrigger to that child — it forwards here.

    private void OnTriggerEnter(Collider other) => OnPlayerDetected(other);
    private void OnTriggerExit(Collider other)  => OnPlayerLost(other);

    public void OnPlayerDetected(Collider other)
    {
        if (other.gameObject.layer != 6) return;

        Target = other.transform;
        TransitionTo(ChaseState);
    }

    public void OnPlayerLost(Collider other)
    {
        if (other.gameObject.layer != 6 || other.transform != Target) return;

        // Don't clear target yet — ChaseState owns the return delay timer.
        // It will transition to ReturnState after returnDelay seconds,
        // and ReturnState will clear the target on exit.
        _currentState?.OnPlayerLeftTrigger();
    }

    // Called by ReturnToOriginState once the enemy is actually heading back
    public void ClearTarget()
    {
        Target = null;
    }

    // ── FSM ───────────────────────────────────────────────────────────────────

    public void TransitionTo(EnemyState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    // ── ICreature ─────────────────────────────────────────────────────────────

    public void SetSpawner(SequentialSpawnerBehaviour spawner)
    {
        _spawner = spawner;
    }

    public void OnHealthDepleted()
    {
        Die();
    }

    private void Die()
    {
        _spawner?.OnCreatureDeath();
        Destroy(gameObject);
    }

    // ── Shared queries ────────────────────────────────────────────────────────

    public bool IsPlayerInShootRange()
    {
        bool isInShootRange  = Target ? Vector3.Distance(transform.position, Target.position) <= shootRange : false;
        return isInShootRange;
    }

    public bool HasReachedWanderLimit()
    {
        Vector3 flatPos = new Vector3(transform.position.x, OriginPosition.y, transform.position.z);
        return Vector3.Distance(flatPos, OriginPosition) >= maxWanderDistance;
    }

    // ── Shared actions ────────────────────────────────────────────────────────

    public void RotateTowardsPlayer()
    {
        if (Target)
        {
            Vector3 direction = Target.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude >= 0.001f) 
                RotateTowards(Quaternion.LookRotation(direction));
        }
    }

    public void RotateTowards(Quaternion targetRotation)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    public void MoveTowardsPlayer()
    {
        if (Target) 
            MoveTowards(Target.position);
    }

    public void MoveTowards(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        direction.y = 0f;
        transform.position += direction * (moveSpeed * Time.deltaTime);
    }

    public void Shoot()
    {
        //Debug.Log("trying to shoot = " + Target.name + "with: " + projectilePrefab.name);
        if (projectilePrefab && Target)
        {
            Vector3 direction = (Target.position - firePoint.position).normalized;
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));

            if (proj.TryGetComponent(out Projectile3D p))
                p.Init(direction, projectileSpeed);
        }
    }

    public void SetRunAnimation(bool running)
    {
        _animator.SetBool(IsRunning, running);
    }

    // ── Editor helpers ────────────────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = Application.isPlaying ? OriginPosition : transform.position;

        // Shoot range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);

        // Wander boundary
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(origin, maxWanderDistance);

        // Note: detection range is visualised by the SphereCollider itself in the editor
    }
}