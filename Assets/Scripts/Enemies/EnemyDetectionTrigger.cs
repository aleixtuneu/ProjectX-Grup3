using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyDetectionTrigger : MonoBehaviour
{
    private EnemyController3D _controller;

    private void Awake()
    {
        _controller = GetComponentInParent<EnemyController3D>();

        if (!_controller)
            Debug.LogError("EnemyDetectionTrigger: no EnemyController3D found in parent.", this);
    }

    private void OnTriggerEnter(Collider other)
    {
        _controller.OnPlayerDetected(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _controller.OnPlayerLost(other);
    }
}