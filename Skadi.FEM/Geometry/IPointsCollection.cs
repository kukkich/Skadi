namespace Skadi.FEM.Geometry;

public interface IPointsCollection<out TPoint>
{
    public int TotalPoints { get; }
    public TPoint this[int index] { get; }
}