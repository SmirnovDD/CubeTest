using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlacedObject : MonoBehaviour, IPlacedObject
{
    [SerializeField] private ObjectToPlaceType _type;
    public ObjectToPlaceType Type => _type;
    
    [SerializeField] private BoxCollider _collider;
    public BoxCollider Collider => _collider;

    [SerializeField] private bool _isSupportBlock;
    public bool IsSupportBlock => _isSupportBlock;
    public int MaxSupportedDistance { get; }

    private MeshRenderer _meshRenderer;
    private Material _defaultMaterial;

    public ReactiveCollection<IPlacedObject> SupportingObjects { get; } = new();

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _defaultMaterial = _meshRenderer.material;
    }

    public void SetMaterial(Material material)
    {
        _meshRenderer.material = material;
    }

    public void SetDefaultMaterial()
    {
        _meshRenderer.material = _defaultMaterial;
    }
    
    public void AddSupportingObject(IPlacedObject supportingObject)
    {
        SupportingObjects.Add(supportingObject);
    }

    public void RemoveSupportingObject(IPlacedObject supportingObject)
    {
        SupportingObjects.Remove(supportingObject);
    }

    private void CheckForSupport()
    {
        
    }
}
