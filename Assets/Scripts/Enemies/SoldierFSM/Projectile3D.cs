using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile3D : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
    }

    public void Init(Vector3 direction, float speed)
    {
        _rb.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 7 && other.gameObject.layer != 2)
        {
            Debug.Log("Collided with = " + other.transform.name + ", layer number = " + other.gameObject.layer);
            if (other.gameObject.layer == 6)
            {
                GameManager.Instance.PlayerDied();
                Destroy(gameObject);
            }
            else if (other.gameObject.layer != gameObject.layer)
            {
                Destroy(gameObject);
            }
        }
    }
}
