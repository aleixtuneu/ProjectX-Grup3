using UnityEngine;
using UnityEngine.InputSystem;

public class ShootBehaviour : MonoBehaviour
{
    public WeaponData currentWeapon;
    public Transform firePoint; 
    public LayerMask enemyLayer; 

    public bool showDebugRays = true;

    private int currentAmmo;
    private float nextFireTime;
    private AudioSource audioSource;
    private Camera playerCamera;
    private PlayerInput playerInput;
    private InputAction attackAction;

    void Start()
    {
        // Inicialitzar
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }   

        // Buscar la càmera
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }       

        if (playerCamera == null)
        {
            Debug.LogError("NO S'HA TROBAT LA CÀMERA! Assigna-la manualment."); //
        }
        else
        {
            Debug.Log($"Càmera trobada: {playerCamera.name}"); //
        }

        // Buscar el FirePoint si no està assignat
        if (firePoint == null)
        {
            // Crear un firePoint automàticament si no existeix
            GameObject fpObj = new GameObject("FirePoint");
            fpObj.transform.SetParent(playerCamera.transform);
            fpObj.transform.localPosition = new Vector3(0.3f, -0.2f, 0.5f); // Davant de la càmera
            firePoint = fpObj.transform;
            Debug.Log("FirePoint creat automàticament");
        }

        // Obtenir PlayerInput i l'acció d'atacar
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            attackAction = playerInput.actions["Attack"];
            Debug.Log("Input System configurat correctament");
        }
        else
        {
            Debug.LogError("NO S'HA TROBAT PlayerInput! Afegeix-lo al Player.");
        }

        if (currentWeapon != null)
        {
            currentAmmo = currentWeapon.maxAmmo;
            Debug.Log($"Arma equipada: {currentWeapon.weaponName} amb {currentAmmo} munició");
        }
        else
        {
            Debug.LogError("NO HI HA ARMA ASSIGNADA! Arrossega un WeaponData al camp 'Current Weapon'");
        }
    }

    void Update()
    {
        // Comprovar si estem atacant amb el New Input System
        if (attackAction != null && attackAction.IsPressed())
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Comprovar si podem disparar
        if (Time.time < nextFireTime)
        {
            return; // No mostrem missatge per no spam
        }

        if (currentAmmo <= 0)
        {
            Debug.Log("Sense munició!");
            return;
        }

        if (currentWeapon == null)
        {
            Debug.LogError("No hi ha arma!");
            return;
        }

        if (playerCamera == null)
        {
            Debug.LogError("No hi ha càmera!");
            return;
        }

        Debug.Log($"DISPARANT! Munició: {currentAmmo}/{currentWeapon.maxAmmo}");

        // Disparar raig des del centre de la càmera
        Vector3 rayOrigin = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;

        // Mostrar raig de debug
        if (showDebugRays)
        {
            Debug.DrawRay(rayOrigin, rayDirection * 100f, Color.yellow, 1f);
        }

        // Raycast
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, 100f))
        {
            Debug.Log($"Impacte en: {hit.collider.gameObject.name} a {hit.distance:F1}m");

            // Intentar fer dany
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(currentWeapon.damage);
                Debug.Log($"{currentWeapon.damage} de dany aplicat a {hit.collider.gameObject.name}!");

                // Línia vermella quan toquem un enemic
                if (showDebugRays)
                    Debug.DrawLine(firePoint.position, hit.point, Color.red, 1f);
            }
            else
            {
                Debug.Log($"{hit.collider.gameObject.name} no pot rebre dany (no té component Enemy o IDamageable)");

                // Línia blava quan toquem algo que no és enemic
                if (showDebugRays)
                    Debug.DrawLine(firePoint.position, hit.point, Color.blue, 1f);
            }
        }
        else
        {
            Debug.Log("No hem tocat res (disparo al buit)");
        }

        // So de disparo
        if (currentWeapon.shootSound != null)
            audioSource.PlayOneShot(currentWeapon.shootSound);

        // Consumir munició i actualitzar temps
        currentAmmo--;
        nextFireTime = Time.time + currentWeapon.fireRate;
    }

    // Mètode públic per canviar d'arma
    public void ChangeWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;
        currentAmmo = newWeapon.maxAmmo;
        Debug.Log($"Arma canviada a: {newWeapon.weaponName}");
    }

    // Recarregar munició
    public void Reload()
    {
        if (currentWeapon != null)
        {
            currentAmmo = currentWeapon.maxAmmo;
            Debug.Log($"Munició recarregada: {currentAmmo}");
        }
    }
}
