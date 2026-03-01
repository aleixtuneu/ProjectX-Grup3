using UnityEngine;

public class TankController : MonoBehaviour
{
    [SerializeField] private WeaponData tankWeapon;
    [SerializeField] private GameObject cannonUI;
    [SerializeField] private float dismountDistance = 2f;

    private bool _isPlayerMounted = false;
    private float _lastShotTime;
    private Transform _playerTransform;
    private InputSystem_Actions _inputActions;
    private Renderer[] _allRenderers;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Enable();
        _allRenderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        if (!_isPlayerMounted) 
            return;

        // Disparar des del tanc
        if (_inputActions.Player.Shoot.WasPressedThisFrame())
        {
            ShootFromTank();
        }

        // Desmuntar amb E
        if (_inputActions.Player.Interact.WasPressedThisFrame())
        {
            Debug.Log("E pressed ON TANK, dismounting...");
            Dismount();
        }
    }

    public void Mount(Transform playerTransform)
    {
        _playerTransform = playerTransform;
        _isPlayerMounted = true;

        // Desactivar renderers del tanc
        foreach (Renderer renderer in _allRenderers)
        {
            renderer.enabled = false;
        }

        // Mostrar canó per pantalla
        if (cannonUI)
            cannonUI.SetActive(true);

        // Desactivar TankInteraction
        GetComponent<TankInteraction>().enabled = false;

        Debug.Log("¡Mounted on tank!");
    }

    public void Dismount()
    {
        if (!_isPlayerMounted) 
            return;

        _isPlayerMounted = false;

        // Moure tanc al costat del jugador
        if (_playerTransform)
        {
            Vector3 dismountPos = _playerTransform.position + _playerTransform.forward * 1f + _playerTransform.right * dismountDistance;
            transform.position = dismountPos;
            Debug.Log($"Tank moved to: {dismountPos}");
        }

        // Activar renderers del tanc
        foreach (Renderer renderer in _allRenderers)
        {
            renderer.enabled = true;
        }

        // Ocultar canó
        if (cannonUI)
            cannonUI.SetActive(false);

        // Reactivar TankInteraction
        GetComponent<TankInteraction>().enabled = true;

        Debug.Log("Dismounting from tank!");
    }

    private void ShootFromTank()
    {
        if (!tankWeapon) 
            return;

        // cadencia
        if (Time.time - _lastShotTime < tankWeapon.fireRate)
            return;

        // Disparar on apunta la càmera
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");

        if (Physics.Raycast(ray, out hit, tankWeapon.raycastDistance, enemyLayer))
        {
            IDamagable damagable = hit.collider.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(tankWeapon.damagePerShot);
                Debug.Log($"Tank shot! Damage: {tankWeapon.damagePerShot}");
            }
        }

        _lastShotTime = Time.time;
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