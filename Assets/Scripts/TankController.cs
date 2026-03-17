using UnityEngine;
using UnityEngine.InputSystem;

public class TankController : MonoBehaviour
{
    [SerializeField] private WeaponData tankWeapon;
    [SerializeField] private GameObject cannonUI;
    [SerializeField] private float dismountDistance = 2f;

    private bool _isPlayerMounted = false;
    private bool _justMounted = false;
    private float _lastShotTime;
    private Transform _playerTransform;
    private InputSystem_Actions _inputActions;
    private Renderer[] _allRenderers;
    private GameObject _cannonObject;

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

        if (_justMounted)
        {
            _justMounted = false;
            return;
        }

        if (_inputActions.Player.Shoot.WasPressedThisFrame())
            ShootFromTank();

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
        _justMounted = true;

        // Buscar Cannon aunque esté inactivo
        Transform cannonTransform = FindInactiveChild(playerTransform, "Cannon");
        if (cannonTransform != null)
        {
            _cannonObject = cannonTransform.gameObject;
            _cannonObject.SetActive(true);
            Debug.Log("Cannon found and activated!");
        }
        else
        {
            Debug.LogWarning("Cannon not found on player!");
        }

        // Desactivar renderers del tanque
        foreach (Renderer renderer in _allRenderers)
            renderer.enabled = false;

        if (cannonUI)
            cannonUI.SetActive(true);

        GetComponent<TankInteraction>().enabled = false;

        Debug.Log("Mounted on tank!");
    }

    public void Dismount()
    {
        if (!_isPlayerMounted)
            return;

        _isPlayerMounted = false;

        // Ocultar cañón
        if (_cannonObject != null)
            _cannonObject.SetActive(false);

        // Mover tanque al lado del jugador
        if (_playerTransform)
        {
            Vector3 dismountPos = _playerTransform.position + _playerTransform.forward * 1f + _playerTransform.right * dismountDistance;
            transform.position = dismountPos;
        }

        // Activar renderers del tanque
        foreach (Renderer renderer in _allRenderers)
            renderer.enabled = true;

        if (cannonUI)
            cannonUI.SetActive(false);

        GetComponent<TankInteraction>().enabled = true;

        Debug.Log("Dismounting from tank!");
    }

    private Transform FindInactiveChild(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name)
                return child;
        }
        return null;
    }

    private void ShootFromTank()
    {
        if (!tankWeapon)
            return;

        if (Time.time - _lastShotTime < tankWeapon.fireRate)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
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

        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(AudioClips.TankShot, transform.position);

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

/*using UnityEngine;
using UnityEngine.InputSystem;

public class TankController : MonoBehaviour
{
    [SerializeField] private WeaponData tankWeapon;
    [SerializeField] private GameObject cannonUI;
    [SerializeField] private GameObject cannonObject;
    [SerializeField] private float dismountDistance = 2f;

    private bool _isPlayerMounted = false;
    private bool _justMounted = false;
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

        // Validar que es la primera vegada que monta
        if (_justMounted)
        {
            _justMounted = false;
            return;
        }

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
        _justMounted = true;

        // Desactivar renderers del tanc
        foreach (Renderer renderer in _allRenderers)
        {
            renderer.enabled = false;
        }

        // Mostrar canó per pantalla
        if (cannonObject)
        {
            cannonObject.SetActive(true);
            Renderer r = cannonObject.GetComponentInChildren<Renderer>();
            if (r) r.enabled = true; // Forzar renderer activo
            Debug.Log($"Renderer enabled: {r?.enabled}");
        }
        else
        {
            Debug.LogWarning("playerCannonObject is NULL!");
        }
        //
        if (cannonUI)
            cannonUI.SetActive(true);
        //

        // Desactivar TankInteraction
        GetComponent<TankInteraction>().enabled = false;

        Debug.Log("Mounted on tank!");
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
        if (cannonObject)
            cannonObject.SetActive(false);
        //
        if (cannonUI)
            cannonUI.SetActive(false);
        //

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
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
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

        // Tank shooting sound
        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(AudioClips.TankShot, transform.position);

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
} */