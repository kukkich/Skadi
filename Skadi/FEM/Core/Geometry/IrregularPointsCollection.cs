namespace Skadi.FEM.Core.Geometry;

public class IrregularPointsCollection<TPoint>(TPoint[] points) : IPointsCollection<TPoint>
{
    public int TotalPoints => _points.Length;
    public TPoint this[int index] => _points[index];
    private TPoint[] _points = points;
}