using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    public ReactiveProperty<bool> IsBlocked = new();
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
        var placedObject = other.GetComponent<IPlacedObject>();
        if (placedObject != null && placedObject.IsGround)
            return;
        if (other.gameObject.layer == LayerMask.NameToLayer("DynamicObject") || SpaceOccupied(other) || IsCombatObject(placedObject))
        {//TODO get state when releasing object set bool to true
            IsBlocked.Value = true;
            _blockingObjects.Add(other);
        }
    }

    private static bool IsCombatObject(IPlacedObject placedObject)
    {
        if (placedObject == null)
            return false;
        return placedObject.IsCombatObject;
    }

    private void OnTriggerStay(Collider other)
    {
        var placedObject = other.GetComponent<IPlacedObject>();
        if (other.gameObject.layer != LayerMask.NameToLayer("DynamicObject") && !SpaceOccupied(other) && !IsCombatObject(placedObject))
        {
            _blockingObjects.Remove(other);
            CheckForBlockRelease();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _colliders.Remove(other);
        _blockingObjects.Remove(other);
        CheckForBlockRelease();
    }

    private void CheckForBlockRelease()
    {
        if(_blockingObjects.Count == 0)
            IsBlocked.Value = false;
    }
    
    private bool SpaceOccupied(Collider other)
    {
        if (Collider.ContainsPoint(other.bounds.center) || other.ContainsPoint(Collider.bounds.center))
            return true;
        return false;
    }
}