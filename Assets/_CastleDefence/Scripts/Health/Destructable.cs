using System;
using UnityEngine;

public class Destructable : MonoBehaviour, IDestructable
{
    private float _durability;
    public float Durability => _durability;
    
    [SerializeField] private float _maxDurability = 100;
    public float MaxDurability => _maxDurability;

    [SerializeField] private bool _isCharacter;
    public bool IsCharacter => _isCharacter;

    private void Awake()
    {
        _durability = MaxDurability;
    }

    public void Damage(float damage)
    {
        _durability -= damage;
        if (_durability <= 0)
            Destroy();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
