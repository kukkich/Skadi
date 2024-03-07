namespace SharpMath.Geometry;

public interface IPointsCollection<out TPoint>
{
    public int Length { get; }
    public TPoint this[int index] { get; }
    public int XLength { get; }
    public int YLength { get; }
}