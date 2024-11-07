namespace SharpMath.FEM.Geometry._2D;

public class IrregularPointsCollection<TPoint> : IPointsCollection<TPoint>
{
    public int TotalPoints => _points.Length;
    public TPoint this[int index] => _points[index];
    
    private readonly TPoint[] _points;
    
    public IrregularPointsCollection(TPoint[] points)
    {
        _points = points;
    }
}