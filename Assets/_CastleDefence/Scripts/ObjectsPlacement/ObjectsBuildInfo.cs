using System.Collections.Generic;

public static class ObjectsBuildInfo
{
    public static Dictionary<ObjectToPlaceType, SnapPointsDirections[]> ObjectsSnapPoints = new()
    {
        {ObjectToPlaceType.SmallCubeWall, new []{SnapPointsDirections.All}},
        {ObjectToPlaceType.SmallCubeFloor, new [] {SnapPointsDirections.Left, SnapPointsDirections.Right, SnapPointsDirections.Front, SnapPointsDirections.Back}},
    };

    public static Dictionary<ObjectToPlaceType, SupportedFromSides[]> ObjectSupportedFromSides = new()
    {
        {ObjectToPlaceType.SmallCubeWall, new[] {SupportedFromSides.Bottom}},
        {ObjectToPlaceType.SmallCubeFloor, new[] {SupportedFromSides.Left, SupportedFromSides.Right, SupportedFromSides.Front, SupportedFromSides.Back}},
    };
}
