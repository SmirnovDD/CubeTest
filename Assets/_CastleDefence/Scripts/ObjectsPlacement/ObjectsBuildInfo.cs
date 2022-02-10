using System.Collections.Generic;

public static class ObjectsBuildInfo
{
    public static Dictionary<ObjectToPlaceType, SnapPointsDirections[]> ObjectsSnapPoints = new()
    {
        {ObjectToPlaceType.SmallCubeWall, new []{SnapPointsDirections.All}},
    };

    public static Dictionary<ObjectToPlaceType, SupportedFromSides[]> ObjectSupportedFromSides = new()
    {
        {ObjectToPlaceType.SmallCubeWall, new[] {SupportedFromSides.Bottom}},
    };
}
