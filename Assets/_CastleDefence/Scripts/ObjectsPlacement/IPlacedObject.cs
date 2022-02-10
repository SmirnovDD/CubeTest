using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface IPlacedObject : IColorChangable
{
    ObjectToPlaceType Type { get; }
    BoxCollider Collider { get; }
    public bool IsSupportBlock { get; }
    public int MaxSupportedDistance { get; }
    public ReactiveCollection <IPlacedObject> SupportingObjects { get; }
    void AddSupportingObject(IPlacedObject connectedObject);
    void RemoveSupportingObject(IPlacedObject connectedObject);
}