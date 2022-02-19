public class NeighbourObject
{
    public IPlacedObject PlacedObject { get; }
    public ConnectedFromSide ConnectedFromSide { get; } 
    public NeighbourObject(IPlacedObject placedObject, ConnectedFromSide connectedFromSide)
    {
        PlacedObject = placedObject;
        ConnectedFromSide = connectedFromSide;
    }
}