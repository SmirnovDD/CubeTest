
public struct RemainingSupportDistance
{
    public SupportedFromSides SupportedFromSides { get; }
    public int Distance { get; }
    
    public RemainingSupportDistance(SupportedFromSides supportedFromSides, int distance)
    {
        SupportedFromSides = supportedFromSides;
        Distance = distance;
    }
}
