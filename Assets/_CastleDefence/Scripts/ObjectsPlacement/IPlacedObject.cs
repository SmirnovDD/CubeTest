using UniRx;
using UnityEngine;

public interface IPlacedObject : IColorChangable
{
    ObjectToPlaceType Type { get; }
    BoxCollider Collider { get; }
    public bool IsSupport { get; }
    public bool IsGround { get; }
    public bool OverrideSnapPosition { get; }
    public ReactiveCollection <NeighbourObject> NeighbourObjects { get; } //TODO maybe change to List<NeighbourObject> to avoid calling ToList()
    void OnPlaced();
    void AddNeighbourObject(NeighbourObject connectedObject);
    void RemoveNeighbourObject(IPlacedObject removedObject);
}
