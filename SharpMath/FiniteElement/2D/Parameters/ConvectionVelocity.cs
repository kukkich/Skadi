using SharpMath.FiniteElement.Core.Assembling.Params;
using SharpMath.Geometry;
using SharpMath.Geometry._2D;
using SharpMath.Geometry._2D.Сylinder;

namespace SharpMath.FiniteElement._2D.Parameters;

public class ConvectionVelocity : IUniversalParameter<Point, Point>
{
    private readonly IPointsCollection<Point> _points;
    private readonly double _maxVelocity;
    private readonly Point _center;
    private readonly double _halfHeight;
    private readonly double _halfRadius;
    
    public ConvectionVelocity(IPointsCollection<Point> points, double maxVelocity)
    {
        _points = points;
        var lb = points[0];
        var rt = points[points.XLength * points.YLength - 1];
        _halfHeight = (rt.Z() - lb.Z()) / 2;
        _halfRadius = (rt.R() - lb.R()) / 2;
        _center = new Point(_halfRadius, _halfHeight);
        _maxVelocity = maxVelocity;
    }
    
    public Point Get(Point point)
    {
        var d = point - _center;
        
        var direction = new Point(-d.Y, d.X).Normalize();

        var scale = d.Norm / new Point(_halfRadius, _halfHeight).Norm;

        return direction * _maxVelocity * scale;
    }
}