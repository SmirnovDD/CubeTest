using System;
using System.Linq;
using UniRx;
using UnityEngine;

public class PlacedObject : MonoBehaviour, IPlacedObject
{
    [SerializeField] private ObjectToPlaceType _type;
    public ObjectToPlaceType Type => _type;
    
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

    private MeshRenderer _meshRenderer;
    private Material _defaultMaterial;

    public ReactiveCollection<NeighbourObject> NeighbourObjects { get; } = new();
    
    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _defaultMaterial = _meshRenderer.material;
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
        _meshRenderer.material = material;
    }

    public void SetDefaultMaterial()
    {
        _meshRenderer.material = _defaultMaterial;
    }
    
    public void AddNeighbourObject(NeighbourObject supportingObject)
    {
        NeighbourObjects.Add(supportingObject);
    }

    public void RemoveNeighbourObject(IPlacedObject supportingObject)
    {
        var neighbour = NeighbourObjects.FirstOrDefault(el => el.PlacedObject == supportingObject);
        if (neighbour != null)
            NeighbourObjects.Remove(neighbour);
    }

    private void CheckForSupport()
    {
        if (!ObjectPlacementUtility.IsObjectSupported(NeighbourObjects.ToList(), _type, _maxSupportedDistance))
            Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        foreach (var neighbourObject in NeighbourObjects)
        {
            Debug.DrawLine(transform.position, neighbourObject.PlacedObject.Collider.transform.position);
        }
    }
}
