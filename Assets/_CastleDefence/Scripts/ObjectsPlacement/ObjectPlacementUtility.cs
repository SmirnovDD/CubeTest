using System;
using System.Collections.Generic;
using System.Linq;
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
        snapPointsDirections = CheckForAllSnapPointsDirections(snapPointsDirections);
        return snapPointsDirections;
    }

    public static SnapPointsDirections[] CheckForAllSnapPointsDirections(SnapPointsDirections[] snapPointsDirections)
    {
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

    public static SupportedFromSides[] CheckForAllSupportedFromSides(SupportedFromSides[] supportedFromSides)
    {
        if (supportedFromSides[0] == SupportedFromSides.All)
        {
            supportedFromSides = new[]
            {
                SupportedFromSides.Bottom, SupportedFromSides.Left, SupportedFromSides.Front,
                SupportedFromSides.Right, SupportedFromSides.Back, SupportedFromSides.Top
            };
        }

        return supportedFromSides;
    }

    public static SupportedFromSides ConnectedFromSideToSupportedFromSides(ConnectedFromSide connectedFromSide)
    {
        switch (connectedFromSide)
        {
            case ConnectedFromSide.Bottom:
                return SupportedFromSides.Bottom;
            case ConnectedFromSide.Left:
                return SupportedFromSides.Left;
            case ConnectedFromSide.Front:
                return SupportedFromSides.Front;
            case ConnectedFromSide.Right:
                return SupportedFromSides.Right;
            case ConnectedFromSide.Back:
                return SupportedFromSides.Back;
            case ConnectedFromSide.Top:
                return SupportedFromSides.Top;
        }

        Debug.LogError("No side found");
        return SupportedFromSides.Bottom;
    }
    
    public static ConnectedFromSide SupportedFromSidesToConnectedFromSide(SupportedFromSides connectedFromSide)
    {
        switch (connectedFromSide)
        {
            case SupportedFromSides.Bottom:
                return ConnectedFromSide.Bottom;
            case SupportedFromSides.Left:
                return ConnectedFromSide.Left;
            case SupportedFromSides.Front:
                return ConnectedFromSide.Front;
            case SupportedFromSides.Right:
                return ConnectedFromSide.Right;
            case SupportedFromSides.Back:
                return ConnectedFromSide.Back;
            case SupportedFromSides.Top:
                return ConnectedFromSide.Top;
        }

        Debug.LogError("No side found");
        return ConnectedFromSide.Bottom;
    }

    public static bool IsObjectSupported(List<NeighbourObject> neighbourObjects, ObjectToPlaceType placedObjectType, int maxSupportedRange, int currentCheckLevel = 0)
    {
        if (currentCheckLevel == maxSupportedRange)
            return false;

        for (int i = 0; i < neighbourObjects.Count; i++)
        {
            var neighbourObj = neighbourObjects[i];
            var neighbourObjectSupportSide = ConnectedFromSideToSupportedFromSides(neighbourObj.ConnectedFromSide);
            var requiredSupportSides = ObjectsBuildInfo.ObjectSupportedFromSides[placedObjectType];
            requiredSupportSides = CheckForAllSupportedFromSides(requiredSupportSides);
            
            if (neighbourObj.PlacedObject.IsSupport && requiredSupportSides.Contains(neighbourObjectSupportSide))
                return true;

            if (IsObjectSupported(neighbourObj.PlacedObject.NeighbourObjects.ToList(), neighbourObj.PlacedObject.Type, maxSupportedRange, currentCheckLevel + 1))
                return true;
        }

        return false;
    }
    
    public static NeighbourObject GetNeighbourObjectFromSide(SupportedFromSides side, List<NeighbourObject> neighbourObjects)
    {
        var connectedFromSide = SupportedFromSidesToConnectedFromSide(side);
        return neighbourObjects.FirstOrDefault(o => o.ConnectedFromSide == connectedFromSide);
    }
    
    private static Vector3 GetSnapPoint(SnapPointsDirections direction, IPlacedObject placingObject)
    {
        var localBounds = new Bounds(placingObject.Collider.center, placingObject.Collider.size);
        var placingObjectTransform = placingObject.Collider.transform;

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

    public static ConnectedFromSide GetConnectedSideFromNeighbourObject(IPlacedObject placedObject, IPlacedObject neighbourObject)
    {
        var backSnapPoint = GetSnapPoint(SnapPointsDirections.Back, placedObject);
        if (ColliderContainsPoint(neighbourObject.Collider, backSnapPoint))
            return ConnectedFromSide.Back;
        
        var frontSnapPoint = GetSnapPoint(SnapPointsDirections.Front, placedObject);
        if (ColliderContainsPoint(neighbourObject.Collider, frontSnapPoint))
            return ConnectedFromSide.Front;
        
        var leftSnapPoint = GetSnapPoint(SnapPointsDirections.Left, placedObject);
        if (ColliderContainsPoint(neighbourObject.Collider, leftSnapPoint))
            return ConnectedFromSide.Left;
        
        var rightSnapPoint = GetSnapPoint(SnapPointsDirections.Right, placedObject);
        if (ColliderContainsPoint(neighbourObject.Collider, rightSnapPoint))
            return ConnectedFromSide.Right;
        
        var topSnapPoint = GetSnapPoint(SnapPointsDirections.Top, placedObject);
        if (ColliderContainsPoint(neighbourObject.Collider, topSnapPoint))
            return ConnectedFromSide.Top;
        
        var bottomSnapPoint = GetSnapPoint(SnapPointsDirections.Bottom, placedObject);
        if (ColliderContainsPoint(neighbourObject.Collider, bottomSnapPoint))
            return ConnectedFromSide.Bottom;
        
        Debug.Log($"{placedObject.Collider.gameObject.name} is not supported by {neighbourObject.Collider.gameObject.name}");
        return ConnectedFromSide.None;
    }
    
    public static bool ColliderContainsPoint(Collider collider, Vector3 point)
    {
        Vector3 localPos = collider.transform.InverseTransformPoint(point);
        if (Mathf.Abs(localPos.x) <= collider.bounds.size.x / 2 && Mathf.Abs(localPos.y) <= collider.bounds.size.y / 2 && Mathf.Abs(localPos.z) <= collider.bounds.size.z / 2)
            return true;
        return false;
    }

}
