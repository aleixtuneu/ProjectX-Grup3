using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Metal Slug/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName = "Pistola";
    public float damage = 10f;
    public float fireRate = 0.2f; // Temps entre dispars
    public int maxAmmo = 200;
    public AudioClip shootSound;
}
