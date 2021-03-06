using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _damage = 20;

    private void OnTriggerEnter(Collider other)
    {
        var destructable = other.GetComponent<IDestructable>();
        destructable?.Damage(_damage);
        SelfDestroy();
    }

    private void SelfDestroy()
    {
        Destroy(gameObject);
    }
}