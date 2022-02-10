using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    public ReactiveProperty<bool> IsColliding = new();
    public Collider Collider { get; private set; }
    private List<Collider> _colliders = new();

    private void Awake()
    {
        Collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        IsColliding.Value = true;
        _colliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _colliders.Remove(other);
        if(_colliders.Count == 0)
            IsColliding.Value = false;
    }
}