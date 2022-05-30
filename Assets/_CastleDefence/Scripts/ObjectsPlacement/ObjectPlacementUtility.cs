using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ObjectPlacementUtility
{
    public static WorldSnapPoint[] GetSnapPointsFromPlacedObject(IPlacedObject objectPlacingOn, IPlacedObject objectToPlace)
    {
        if (objectPlacingOn == null)
        {
            Debug.Log("Object placing on is null");
            return Array.Empty<WorldSnapPoint>();
        }
        if (!ObjectsBuildInfo.ObjectsDefaultSnapPoints.ContainsKey(objectPlacingOn.Type) && !ObjectsBuildInfo.ObjectsOverridenSnapPoints.ContainsKey(objectPlacingOn.Type))
        {
            Debug.Log($"{objectPlacingOn.Type} not found in dictionary");
            return Array.Empty<WorldSnapPoint>();
        }
        if (objectToPlace == null)
        {
            Debug.Log("Object to place is null");
            return Array.Empty<WorldSnapPoint>();
        }  
        var snapPointsDirections = GetSnapPointsDirections(objectPlacingOn.Type, objectToPlace.OverrideSnapPosition);
        WorldSnapPoint[] snapPoints = new WorldSnapPoint[snapPointsDirections.Length];

        for (var i = 0; i < snapPoints.Length; i++)
        {
            snapPoints[i] = new WorldSnapPoint(snapPointsDirections[i], GetSnapPoint(snapPointsDirections[i], objectPlacingOn, objectToPlace));
        }

        return snapPoints;
    }

    private static SnapPointsDirections[] GetSnapPointsDirections(ObjectToPlaceType type, bool overridenPoints)
    {
        SnapPointsDirections[] snapPointsDirections;
        
        if (overridenPoints && ObjectsBuildInfo.ObjectsOverridenSnapPoints.ContainsKey(type))
            snapPointsDirections = ObjectsBuildInfo.ObjectsOverridenSnapPoints[type];
        else
            snapPointsDirections = ObjectsBuildInfo.ObjectsDefaultSnapPoints[type];
        
        snapPointsDirections = CheckForAllSnapPointsDirections(snapPointsDirections);
        return snapPointsDirections;
    }

    private static SnapPointsDirections[] CheckForAllSnapPointsDirections(SnapPointsDirections[] snapPointsDirections)
    {
        if (snapPointsDirections[0] == SnapPointsDirections.AllCentered)
        {
            snapPointsDirections = new[]
            {
                SnapPointsDirections.Bottom, SnapPointsDirections.Left, SnapPointsDirections.Front,
                SnapPointsDirections.Right, SnapPointsDirections.Back, SnapPointsDirections.Top
            };
        }

        if (snapPointsDirections[0] == SnapPointsDirections.TopAndBottomOnSides)
        {
            snapPointsDirections = new[]
            {
                SnapPointsDirections.BackBottom, SnapPointsDirections.BackTop, SnapPointsDirections.FrontBottom, SnapPointsDirections.FrontTop,
                SnapPointsDirections.LeftBottom, SnapPointsDirections.LeftTop, SnapPointsDirections.RightBottom, SnapPointsDirections.RightTop
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

        if (placedObjectType == ObjectToPlaceType.Ground)
            return true;
        
        for (int i = 0; i < neighbourObjects.Count; i++)
        {
            var neighbourObj = neighbourObjects[i];

            if (neighbourObj.PlacedObject == null || neighbourObj.PlacedObject.Collider == null)
                continue;
            
            var neighbourObjectSupportSide = ConnectedFromSideToSupportedFromSides(neighbourObj.ConnectedFromSide);
            var requiredSupportSides = ObjectsBuildInfo.ObjectSupportedFromSides[placedObjectType];
            requiredSupportSides = CheckForAllSupportedFromSides(requiredSupportSides);

            if (!requiredSupportSides.Contains(neighbourObjectSupportSide))
                continue;
            
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
    
    private static Vector3 GetSnapPoint(SnapPointsDirections direction, IPlacedObject objectPlacingOn, IPlacedObject objectToPlace)
    {
        var localBounds = new Bounds(objectPlacingOn.Collider.center, objectPlacingOn.Collider.size);
        var placingObjectTransform = objectPlacingOn.Collider.transform;
        var objectToPlaceHeight = objectToPlace.Collider.size.y;
        var offsetType = ObjectsBuildInfo.ObjectSnapOffsets[objectToPlace.Type];
        float y;
        
        switch (direction)
        {
            case SnapPointsDirections.Bottom:
                return placingObjectTransform.TransformPoint(new Vector3(0, localBounds.min.y, 0));
            case SnapPointsDirections.Left:
                return placingObjectTransform.TransformPoint(new Vector3(localBounds.min.x, 0, 0));
            case SnapPointsDirections.LeftTop:
                y = GetSnapPointYWithOffset(direction, offsetType, localBounds.max.y, objectToPlaceHeight);
                return placingObjectTransform.TransformPoint(new Vector3(localBounds.min.x, y, 0));
            case SnapPointsDirections.LeftBottom:
                y = GetSnapPointYWithOffset(direction, offsetType, localBounds.min.y, objectToPlaceHeight);
                return placingObjectTransform.TransformPoint(new Vector3(localBounds.min.x, y, 0));
            case SnapPointsDirections.Front:
                return placingObjectTransform.TransformPoint(new Vector3(0, 0, localBounds.max.z));
            case SnapPointsDirections.FrontTop:
                y = GetSnapPointYWithOffset(direction, offsetType, localBounds.max.y, objectToPlaceHeight);
                return placingObjectTransform.TransformPoint(new Vector3(0, y, localBounds.max.z));
            case SnapPointsDirections.FrontBottom:
                y = GetSnapPointYWithOffset(direction, offsetType, localBounds.min.y, objectToPlaceHeight);
                return placingObjectTransform.TransformPoint(new Vector3(0, y, localBounds.max.z));
            case SnapPointsDirections.Right:
                return placingObjectTransform.TransformPoint(new Vector3(localBounds.max.x, 0, 0));
            case SnapPointsDirections.RightTop:
                y = GetSnapPointYWithOffset(direction, offsetType, localBounds.max.y, objectToPlaceHeight);
                return placingObjectTransform.TransformPoint(new Vector3(localBounds.max.x, y, 0));
            case SnapPointsDirections.RightBottom:
                y = GetSnapPointYWithOffset(direction, offsetType, localBounds.min.y, objectToPlaceHeight);
                return placingObjectTransform.TransformPoint(new Vector3(localBounds.max.x, y, 0));
            case SnapPointsDirections.Back:
                return placingObjectTransform.TransformPoint(new Vector3(0, 0, localBounds.min.z));
            case SnapPointsDirections.BackTop:
                y = GetSnapPointYWithOffset(direction, offsetType, localBounds.max.y, objectToPlaceHeight);
                return placingObjectTransform.TransformPoint(new Vector3(0, y, localBounds.min.z));
            case SnapPointsDirections.BackBottom:
                y = GetSnapPointYWithOffset(direction, offsetType, localBounds.min.y, objectToPlaceHeight);
                return placingObjectTransform.TransformPoint(new Vector3(0, y, localBounds.min.z));
            case SnapPointsDirections.Top:
                return placingObjectTransform.TransformPoint(new Vector3(0, localBounds.max.y, 0));
        }

        return Vector3.zero;
    }

    private static float GetSnapPointYWithOffset(SnapPointsDirections direction, ObjectSnapOffset offset, float snapPointY, float objectHeight)
    {
        if (offset != ObjectSnapOffset.HalfHeightOffset)
            return snapPointY;
        
        switch (direction)
        {
            case SnapPointsDirections.LeftTop:
            case SnapPointsDirections.FrontTop:
            case SnapPointsDirections.RightTop:
            case SnapPointsDirections.BackTop:
                return snapPointY - objectHeight / 2;
            case SnapPointsDirections.LeftBottom:
            case SnapPointsDirections.FrontBottom:
            case SnapPointsDirections.RightBottom:
            case SnapPointsDirections.BackBottom:
                return snapPointY + objectHeight / 2;
        }
        
        return snapPointY;
    }

    public static ConnectedFromSide GetConnectedSideFromNeighbourObject(IPlacedObject placedObject, IPlacedObject neighbourObject)
    {
        var topSnapPoint = GetSnapPoint(SnapPointsDirections.Top, placedObject, neighbourObject);
        if (ContainsPoint(neighbourObject.Collider, topSnapPoint))
            return ConnectedFromSide.Top;
        
        var bottomSnapPoint = GetSnapPoint(SnapPointsDirections.Bottom, placedObject, neighbourObject);
        if (ContainsPoint(neighbourObject.Collider, bottomSnapPoint))
            return ConnectedFromSide.Bottom;
        
        var backSnapPoint = GetSnapPoint(SnapPointsDirections.Back, placedObject, neighbourObject);
        if (ContainsPoint(neighbourObject.Collider, backSnapPoint))
            return ConnectedFromSide.Back;

        var backTopSnapPoint = GetSnapPoint(SnapPointsDirections.BackTop, placedObject, neighbourObject);
        if (ContainsPoint(neighbourObject.Collider, backTopSnapPoint))
            return ConnectedFromSide.Back;

        var backBottomPoint = GetSnapPoint(SnapPointsDirections.BackBottom, placedObject, neighbourObject);
        if (ContainsPoint(neighbourObject.Collider, backBottomPoint))
            return ConnectedFromSide.Back;

        var frontSnapPoint = GetSnapPoint(SnapPointsDirections.Front, placedObject, neighbourObject);
        if (ContainsPoint(neighbourObject.Collider, frontSnapPoint))
            return ConnectedFromSide.Front;

        var frontTopSnapPoint = GetSnapPoint(SnapPointsDirections.FrontTop, placedObject, neighbourObject);
        if (ContainsPoint(neighbourObject.Collider, frontTopSnapPoint))
            return ConnectedFromSide.Front;

        var frontBottomSnapPoint = GetSnapPoint(SnapPointsDirections.FrontBottom, placedObject, neighbourObject);
        if (ContainsPoint(neighbourObject.Collider, frontBottomSnapPoint))
            return ConnectedFromSide.Front;

        var leftSnapPoint = GetSnapPoint(SnapPointsDirections.Left, placedObject, neighbourObject);
        if (ContainsPoint(neighbourObject.Collider, leftSnapPoint))
            return ConnectedFromSide.Left;

        var leftTopSnapPoint = GetSnapPoint(SnapPointsDirections.LeftTop, placedObject, neighbourObject);
        if (ContainsPoint(neighbourObject.Collider, leftTopSnapPoint))
            return ConnectedFromSide.Left;

        var leftBottomSnapPoint = GetSnapPoint(SnapPointsDirections.LeftBottom, placedObject, neighbourObject);
        if (ContainsPoint(neighbourObject.Collider, leftBottomSnapPoint))
            return ConnectedFromSide.Left;

        var rightSnapPoint = GetSnapPoint(SnapPointsDirections.Right, placedObject, neighbourObject);
        if (ContainsPoint(neighbourObject.Collider, rightSnapPoint))
            return ConnectedFromSide.Right;

        var rightTopSnapPoint = GetSnapPoint(SnapPointsDirections.RightTop, placedObject, neighbourObject);
        if (ContainsPoint(neighbourObject.Collider, rightTopSnapPoint))
            return ConnectedFromSide.Right;

        var rightBottomSnapPoint = GetSnapPoint(SnapPointsDirections.RightBottom, placedObject, neighbourObject);
        if (ContainsPoint(neighbourObject.Collider, rightBottomSnapPoint))
            return ConnectedFromSide.Right;
        
        Debug.Log($"{placedObject.Collider.gameObject.name} is not supported by {neighbourObject.Collider.gameObject.name}");
        return ConnectedFromSide.None;
    }
    
    public static bool ContainsPoint(this Collider collider, Vector3 point)
    {
        Vector3 localPos = collider.transform.InverseTransformPoint(point);
        var boundsSize = collider.bounds.size;
        var isXPosInside = LessOrEqualNoFloatError(Mathf.Abs(localPos.x),boundsSize.x / 2);
        var isYPosInside = LessOrEqualNoFloatError(Mathf.Abs(localPos.y),boundsSize.y / 2);
        var isZPosInside = LessOrEqualNoFloatError(Mathf.Abs(localPos.z),boundsSize.z / 2);
        if (isXPosInside && isYPosInside && isZPosInside)
            return true;
        return false;
    }


    private static bool LessOrEqualNoFloatError(float a, float b)
    {
        if (a < b)
            return true;
        if (Mathf.Approximately(a, b))
            return true;
        return false;
    }

    public static bool CheckForSupportRule(IPlacedObject placedObject, SnapPointsDirections placedObjectSnapPoint, SnapPointsDirections neighbourObjectSnapPoint)
    {
        var supportRule = ObjectsBuildInfo.ObjectSupportRule[placedObject.Type];
        if (supportRule == SupportRule.HorizontalPlaneOppositeSides)
            return CheckForHorizontalPlaneOppositeSidesRule(placedObjectSnapPoint, neighbourObjectSnapPoint);
        if (supportRule == SupportRule.All)
            return true;
        Debug.LogError("No rule found");
        return false;
    }

    private static bool CheckForHorizontalPlaneOppositeSidesRule(SnapPointsDirections dirA, SnapPointsDirections dirB)
    {
        if (dirA == SnapPointsDirections.Top || dirA == SnapPointsDirections.Bottom ||
            dirB == SnapPointsDirections.Top || dirB == SnapPointsDirections.Bottom)
            return false;
        
        return true;
    }
}
