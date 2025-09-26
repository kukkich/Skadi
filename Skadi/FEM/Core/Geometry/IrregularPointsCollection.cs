namespace Skadi.FEM.Core.Geometry;

public class IrregularPointsCollection<TPoint>(TPoint[] points) : IPointsCollection<TPoint>
{
    public int TotalPoints => points.Length;
    public TPoint this[int index] => points[index];
}