using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [SerializeField] public string weaponName = "Default Gun";
    [SerializeField] public int damagePerShot = 10;
    [SerializeField] public float fireRate = 0.2f;
    [SerializeField] public float raycastDistance = 100f;
}
