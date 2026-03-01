using UnityEngine;

public class ShootBehaviour : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private WeaponData weapon;
    [SerializeField] private Transform shootOrigin;

    private InputSystem_Actions _inputActions;
    private float _lastShotTime;

    private void Awake()
    {
        if (!shootOrigin)
            shootOrigin = transform;

        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Enable();
    }

    private void Update()
    {
        if (_inputActions.Player.Shoot.IsPressed())
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (Time.time - _lastShotTime < weapon.fireRate)
            return;

        Shoot();
        _lastShotTime = Time.time;
    }

    private void Shoot()
    {
        Vector3 origin = shootOrigin.position;
        Vector3 direction = shootOrigin.forward;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, weapon.raycastDistance, enemyLayer))
        {
            IDamagable damagable = hit.collider.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(weapon.damagePerShot);
                Debug.Log($"¡Hitted {hit.collider.name}!");
            }
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