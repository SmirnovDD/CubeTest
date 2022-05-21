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
        if (other.gameObject.layer == LayerMask.NameToLayer("PlacingObjectCollisionCheck") ||
            other.gameObject.layer == LayerMask.NameToLayer("DynamicObject") || SpaceOccupied(other))
        {
            IsCollidingWithBlockingObject.Value = true;
            _blockingObjects.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _colliders.Remove(other);
        if (_blockingObjects.Contains(other))
            _blockingObjects.Remove(other);
        if(_blockingObjects.Count == 0)
            IsCollidingWithBlockingObject.Value = false;
    }

    private bool SpaceOccupied(Collider other)
    {
        if (Collider.ContainsPoint(other.bounds.center) || other.ContainsPoint(Collider.bounds.center))
            return true;
        return false;
    }
}