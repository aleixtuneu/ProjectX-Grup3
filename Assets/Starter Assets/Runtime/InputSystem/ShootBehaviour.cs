using UnityEngine;

/// <summary>
/// Handles shooting logic for the equipped weapon.
/// All dependencies are explicit — assign them in the Inspector.
/// Input is never read here; call <see cref="TryShoot"/> from your input handler.
/// </summary>
public class ShootBehaviour : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------
    // Inspector
    // -----------------------------------------------------------------------------------------

    [Header("Weapon")]
    [SerializeField] private WeaponData currentWeapon;

    [Header("References")]
    [SerializeField] private Camera    playerCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Debug")]
    [SerializeField] private bool showDebugRays = true;

    // -----------------------------------------------------------------------------------------
    // State
    // -----------------------------------------------------------------------------------------

    private int         _currentAmmo;
    private float       _nextFireTime;
    private AudioSource _audioSource;

    // -----------------------------------------------------------------------------------------
    // Unity lifecycle
    // -----------------------------------------------------------------------------------------

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (currentWeapon != null)
            _currentAmmo = currentWeapon.maxAmmo;
        else
            Debug.LogError($"{nameof(ShootBehaviour)}: no WeaponData assigned.", this);
    }

    // -----------------------------------------------------------------------------------------
    // Public API
    // -----------------------------------------------------------------------------------------

    /// <summary>
    /// Attempts to fire. Silently returns if on cooldown or out of ammo.
    /// Called by the input handler on Attack performed.
    /// </summary>
    public void TryShoot()
    {
        if (!CanShoot()) return;

        PerformShot();
    }

    /// <summary>Swaps the equipped weapon and resets ammo to the new weapon's maximum.</summary>
    public void ChangeWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;
        _currentAmmo  = newWeapon.maxAmmo;
    }

    /// <summary>Refills ammo to the current weapon's maximum.</summary>
    public void Reload()
    {
        if (currentWeapon != null)
            _currentAmmo = currentWeapon.maxAmmo;
    }

    // -----------------------------------------------------------------------------------------
    // Private helpers
    // -----------------------------------------------------------------------------------------

    private bool CanShoot()
    {
        if (currentWeapon == null)     return false;
        if (Time.time < _nextFireTime) return false;
        if (_currentAmmo <= 0)
        {
            Debug.Log("Out of ammo.");
            return false;
        }
        return true;
    }

    private void PerformShot()
    {
        bool hitDamageable = TryHitTarget(out RaycastHit hitInfo);

        DrawDebugRays(hitDamageable, hitInfo);

        if (currentWeapon.shootSound != null)
            _audioSource.PlayOneShot(currentWeapon.shootSound);

        _currentAmmo--;
        _nextFireTime = Time.time + currentWeapon.fireRate;
    }

    private bool TryHitTarget(out RaycastHit hitInfo)
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (!Physics.Raycast(ray, out hitInfo, maxDistance: 100f))
            return false;

        IDamageable target = hitInfo.collider.GetComponent<IDamageable>();
        target?.TakeDamage(currentWeapon.damage);

        return target != null;
    }

    private void DrawDebugRays(bool hitDamageable, RaycastHit hitInfo)
    {
        if (!showDebugRays) return;

        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 100f, Color.yellow, 1f);

        if (hitInfo.collider != null)
        {
            Color lineColor = hitDamageable ? Color.red : Color.blue;
            Debug.DrawLine(firePoint.position, hitInfo.point, lineColor, 1f);
        }
    }
}
