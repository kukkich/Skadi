using SharpMath.Geometry._2D;

namespace SharpMath.FEM.Geometry._2D;

public class IrregularPointsCollection : IPointsCollection<Point2D>
{
    public int TotalPoints => _points.Length;
    public Point2D this[int index] => _points[index];
    
    private readonly Point2D[] _points;
    
    public IrregularPointsCollection(Point2D[] points)
    {
        _points = points;
    }
}