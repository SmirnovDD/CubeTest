using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    public ReactiveProperty<bool> IsCollidingWithBlockingObject = new();
    public Collider Collider { get; private set; }
    
    private readonly List<Collider> _colliders = new();
    public List<Collider> Colliders => _colliders;
    
    private readonly List<Collider> _blockingObjects = new ();
    
    private void Awake()
    {
        Collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _colliders.Add(other);
        if (other.gameObject.layer == LayerMask.NameToLayer("PlacingObjectCollisionCheck"))
        {
            IsCollidingWithBlockingObject.Value = true;
            _blockingObjects.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _colliders.Remove(other);
        if (other.gameObject.layer == LayerMask.NameToLayer("PlacingObjectCollisionCheck"))
            _blockingObjects.Remove(other);
        if(_blockingObjects.Count == 0)
            IsCollidingWithBlockingObject.Value = false;
    }
}