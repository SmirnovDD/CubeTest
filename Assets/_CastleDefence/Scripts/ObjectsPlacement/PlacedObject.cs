using System;
using System.Linq;
using UniRx;
using UnityEngine;

public class PlacedObject : MonoBehaviour, IPlacedObject
{
    [SerializeField] private ObjectToPlaceType _type;
    public ObjectToPlaceType Type => _type;

    [SerializeField] private Collider _colliderToSwap;
    public Collider ColliderToSwap => _colliderToSwap;

    [SerializeField] private BoxCollider _collider;
    public BoxCollider Collider => _collider;

    [SerializeField] private bool _isSupport;
    public bool IsSupport => _isSupport;

    [SerializeField] private bool _isGround;
    public bool IsGround => _isGround;

    [SerializeField] private bool _overrideSnapPosition;
    public bool OverrideSnapPosition => _overrideSnapPosition;

    [SerializeField] private int _maxSupportedDistance;
    public int MaxSupportedDistance => _maxSupportedDistance;

    private MeshRenderer[] _meshRenderers;
    private Material[] _defaultMaterials;

    public bool HasNeighbourFromSide(ConnectedFromSide side)
    {
        if (NeighbourObjects.Count == 0)
            return false;
        return NeighbourObjects.FirstOrDefault(no => no.ConnectedFromSide == side) != null;
    }

    public ReactiveCollection<NeighbourObject> NeighbourObjects { get; } = new();
    
    private void Awake()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _defaultMaterials = new Material[_meshRenderers.Length];
        
        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            _defaultMaterials[i] = _meshRenderers[i].material;
        }
        NeighbourObjects.ObserveRemove().Subscribe(x => CheckForSupport()).AddTo(this);
    }

    private void OnDestroy()
    {
        for (var i = NeighbourObjects.Count - 1; i >= 0; i--)
        {
            var neighbourObject = NeighbourObjects[i];
            neighbourObject.PlacedObject.RemoveNeighbourObject(this);
        }
    }

    public void OnPlaced()
    {
        CheckForSupport();
    }

    public void SetMaterial(Material material)
    {
        foreach (var meshRenderer in _meshRenderers)
        {
            meshRenderer.material = material;
        }
    }

    public void SetDefaultMaterial()
    {
        for (var i = 0; i < _meshRenderers.Length; i++)
        {
            var meshRenderer = _meshRenderers[i];
            meshRenderer.material = _defaultMaterials[i];
        }
    }
    
    public void AddNeighbourObject(NeighbourObject supportingObject)
    {
        NeighbourObjects.Add(supportingObject);
    }

    public void RemoveNeighbourObject(IPlacedObject removedObject)
    {
        var neighbour = NeighbourObjects.FirstOrDefault(el => el.PlacedObject == removedObject);
        if (neighbour != null)
            NeighbourObjects.Remove(neighbour);
    }

    public void CheckForSupport()
    {
        if (!ObjectPlacementUtility.IsObjectSupported(NeighbourObjects.ToList(), _type, _maxSupportedDistance))
            Destroy(gameObject);
    }
}
