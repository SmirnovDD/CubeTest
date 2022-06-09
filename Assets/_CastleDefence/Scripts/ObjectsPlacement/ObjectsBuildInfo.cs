using System.Collections.Generic;

public static class ObjectsBuildInfo
{
    public static readonly Dictionary<ObjectToPlaceType, SnapPointsDirections[]> ObjectsDefaultSnapPoints = new()
    {
        {ObjectToPlaceType.Ground, new []{SnapPointsDirections.AllCentered}},
        {ObjectToPlaceType.SmallCubeWall, new []{SnapPointsDirections.AllCentered}},
        {ObjectToPlaceType.SmallCubeFloor, new [] {SnapPointsDirections.Front, SnapPointsDirections.Left, SnapPointsDirections.Back, SnapPointsDirections.Right}},
        {ObjectToPlaceType.SmallCubeStairs, new [] {SnapPointsDirections.AllCentered}},
        {ObjectToPlaceType.Door, new [] {SnapPointsDirections.AllCentered}},
    };

    public static readonly Dictionary<ObjectToPlaceType, SnapPointsDirections[]> ObjectsOverridenSnapPoints = new()
    {
        {ObjectToPlaceType.SmallCubeWall, new []{SnapPointsDirections.TopAndBottomOnSides}},
    };

    public static readonly Dictionary<ObjectToPlaceType, SupportedFromSides[]> ObjectSupportedFromSides = new()
    {
        {ObjectToPlaceType.Ground, new[] {SupportedFromSides.Bottom}},
        {ObjectToPlaceType.SmallCubeWall, new[] {SupportedFromSides.Bottom}},
        {ObjectToPlaceType.SmallCubeFloor, new[] {SupportedFromSides.Left, SupportedFromSides.Right, SupportedFromSides.Front, SupportedFromSides.Back}},
        {ObjectToPlaceType.SmallCubeStairs, new[] {SupportedFromSides.Bottom}},
        {ObjectToPlaceType.Door, new[] {SupportedFromSides.Bottom, SupportedFromSides.Left, SupportedFromSides.Right}},
    };

    public static readonly Dictionary<ObjectToPlaceType, SupportRule> ObjectSupportRule = new()
    {
        {ObjectToPlaceType.Ground, SupportRule.All},
        {ObjectToPlaceType.SmallCubeWall, SupportRule.All},
        {ObjectToPlaceType.SmallCubeFloor, SupportRule.HorizontalPlaneOppositeSides},
        {ObjectToPlaceType.SmallCubeStairs, SupportRule.All},
        {ObjectToPlaceType.Door, SupportRule.Top},
    };

    public static readonly Dictionary<ObjectToPlaceType, ObjectSnapOffset> ObjectSnapOffsets = new()
    {
        {ObjectToPlaceType.Ground, ObjectSnapOffset.None},
        {ObjectToPlaceType.SmallCubeWall, ObjectSnapOffset.None},
        {ObjectToPlaceType.SmallCubeFloor, ObjectSnapOffset.HalfHeightOffset},
        {ObjectToPlaceType.SmallCubeStairs, ObjectSnapOffset.None},
        {ObjectToPlaceType.Door, ObjectSnapOffset.None},
    };
}
