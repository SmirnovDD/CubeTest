using System;
using UnityEngine;

public static class ObjectPlacementUtility
{
    public static Vector3[] GetSnapPointsFromPlacedObject(PlacedObject placingObject)
    {
        if (!ObjectsBuildInfo.ObjectsSnapPoints.ContainsKey(placingObject.Type))
        {
            Debug.Log($"{placingObject.Type} not found in dictionary");
            return Array.Empty<Vector3>();
        }

        var snapPointsDirections = GetSnapPointsDirections(placingObject.Type);
        Vector3[] snapPoints = new Vector3[snapPointsDirections.Length];

        for (var i = 0; i < snapPoints.Length; i++)
        {
            snapPoints[i] = GetSnapPoint(snapPointsDirections[i], placingObject);
        }

        return snapPoints;
    }

    private static SnapPointsDirections[] GetSnapPointsDirections(ObjectToPlaceType type)
    {
        var snapPointsDirections = ObjectsBuildInfo.ObjectsSnapPoints[type];
        if (snapPointsDirections[0] == SnapPointsDirections.All)
        {
            snapPointsDirections = new[]
            {
                SnapPointsDirections.Bottom, SnapPointsDirections.Left, SnapPointsDirections.Front,
                SnapPointsDirections.Right, SnapPointsDirections.Back, SnapPointsDirections.Top
            };
        }
        return snapPointsDirections;
    }

    private static Vector3 GetSnapPoint(SnapPointsDirections direction, PlacedObject placingObject)
    {
        var localBounds = new Bounds(placingObject.Collider.center, placingObject.Collider.size);
        var placingObjectTransform = placingObject.transform;

        switch (direction)
        {
            case SnapPointsDirections.Bottom:
                return placingObjectTransform.TransformPoint(new Vector3(0, localBounds.min.y, 0));
            case SnapPointsDirections.Left:
                return placingObjectTransform.TransformPoint(new Vector3(localBounds.min.x, 0, 0));
            case SnapPointsDirections.Front:
                return placingObjectTransform.TransformPoint(new Vector3(0, 0, localBounds.max.z));
            case SnapPointsDirections.Right:
                return placingObjectTransform.TransformPoint(new Vector3(localBounds.max.x, 0, 0));
            case SnapPointsDirections.Back:
                return placingObjectTransform.TransformPoint(new Vector3(0, 0, localBounds.min.z));
            case SnapPointsDirections.Top:
                return placingObjectTransform.TransformPoint(new Vector3(0, localBounds.max.y, 0));
        }

        return Vector3.zero;
    }
}
